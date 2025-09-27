using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using PEAKLib.UI.Elements;
using PEAKLib.UI;

namespace PeakChatOps.Core;

// Minimal PeakText pool to reduce frequent Create/Destroy of PeakText instances.
// Usage: create one pool (optionally with a template), call Get(text) to obtain an instance,
// and Release(instance) when done (we keep instances inactive instead of destroying).
public class PeakTextPool
{
	private readonly Queue<PeakText> pool = new Queue<PeakText>();
	// track instances that already had TMP settings applied to avoid repeated work
	private readonly HashSet<int> appliedSettings = new HashSet<int>();
	private readonly Transform poolContainerTransform;
	private PeakText template = null!;
	private readonly int maxPoolSize;
	// cache reflection info for textWrappingMode to avoid repeated GetProperty calls
	private static System.Reflection.PropertyInfo textWrappingModeProp = null;
	// cache reflection info for legacy enableWordWrapping to avoid direct deprecated member access
	private static System.Reflection.PropertyInfo enableWordWrappingProp = null;

	/// <summary>
	/// Create a pool. If template is provided, instances will be instantiated from it.
	/// poolContainerParent is used to parent idle objects; if null a hidden container under root will be created.
	/// maxPoolSize controls how many instances will be kept in the pool; excess releases will be destroyed.
	/// </summary>
	public PeakTextPool(Transform poolContainerParent, PeakText template = null!, int maxPoolSize = 50, string poolContainerName = "PeakTextPool", int warmupCount = 0)
	{
		this.template = template;
		this.maxPoolSize = Mathf.Max(1, maxPoolSize);

		// Create a dedicated container for pooled objects to keep hierarchy tidy
		GameObject containerGO = new GameObject(poolContainerName);
		containerGO.SetActive(false);
		if (poolContainerParent != null)
		{
			try { containerGO.transform.SetParent(poolContainerParent, false); } catch { }
		}
		this.poolContainerTransform = containerGO.transform;

		// If we have a template, apply TMP settings once to it so instantiated clones inherit desired properties
		if (this.template != null)
		{
			try
			{
				if (!appliedSettings.Contains(this.template.GetInstanceID()))
				{
					ApplyTMPSettings(this.template);
					appliedSettings.Add(this.template.GetInstanceID());
				}
			}
			catch { }
		}
		else
		{
			// If no template was provided, try to create one via MenuAPI as a best-effort.
			try
			{
				var created = MenuAPI.CreateText("");
				if (created != null)
				{
					// Parent to pool container and deactivate so clones can be made from it
					try { created.transform.SetParent(this.poolContainerTransform, false); } catch { }
					try { created.gameObject.SetActive(false); } catch { }
					this.template = created;
					try
					{
						if (!appliedSettings.Contains(this.template.GetInstanceID()))
						{
							ApplyTMPSettings(this.template);
							appliedSettings.Add(this.template.GetInstanceID());
						}
					}
					catch { }
					DevLog.File("[DEBUG] PeakTextPool: Created template via MenuAPI.CreateText");
				}
				else
				{
					DevLog.File("[DEBUG] PeakTextPool: No template provided and MenuAPI.CreateText returned null");
				}
			}
			catch (System.Exception ex)
			{
				DevLog.File($"[DEBUG] PeakTextPool: Exception creating template via MenuAPI: {ex.GetType().Name}: {ex.Message}");
			}
		}

		// Do not warmup synchronously in constructor (MenuAPI or TMP may not be ready yet).
		// Warmup should be invoked explicitly when the environment is ready.
		try
		{
			// noop here - caller may call Warmup later (we keep parameter for compatibility)
		}
		catch { }
	}

	// Attempt to ensure we have a usable template. This can be called lazily from Get
	// if the constructor ran too early for MenuAPI to produce a good template.
	private void EnsureTemplate()
	{
		if (this.template != null) return;
		try
		{
			var created = MenuAPI.CreateText("");
			if (created != null)
			{
				try { created.transform.SetParent(this.poolContainerTransform, false); } catch { }
				try { created.gameObject.SetActive(false); } catch { }
				this.template = created;
				try
				{
					if (!appliedSettings.Contains(this.template.GetInstanceID()))
					{
						ApplyTMPSettings(this.template);
						appliedSettings.Add(this.template.GetInstanceID());
					}
				}
				catch { }
				DevLog.File("[DEBUG] PeakTextPool: Created template via MenuAPI.CreateText (EnsureTemplate)");
			}
			else
			{
				DevLog.File("[DEBUG] PeakTextPool: EnsureTemplate - MenuAPI.CreateText returned null");
			}
		}
		catch (System.Exception ex)
		{
			DevLog.File($"[DEBUG] PeakTextPool: Exception in EnsureTemplate: {ex.GetType().Name}: {ex.Message}");
		}
	}

	// Async variant that will retry MenuAPI.CreateText across multiple frames.
	// Returns true when a usable template was created or already existed, false otherwise.
	public async UniTask<bool> EnsureTemplateAsync(int maxAttempts = 8, int delayMs = 80)
	{
		if (this.template != null) return true;
		for (int attempt = 0; attempt < maxAttempts; attempt++)
		{
			try
			{
				var created = MenuAPI.CreateText("");
				if (created != null)
				{
					try { created.transform.SetParent(this.poolContainerTransform, false); } catch { }
					try { created.gameObject.SetActive(false); } catch { }
					this.template = created;
					try
					{
						if (!appliedSettings.Contains(this.template.GetInstanceID()))
						{
							ApplyTMPSettings(this.template);
							appliedSettings.Add(this.template.GetInstanceID());
						}
					}
					catch { }
					DevLog.File($"[DEBUG] PeakTextPool: Created template via MenuAPI.CreateText (EnsureTemplateAsync) after attempt {attempt + 1}");
					return true;
				}
				else
				{
					DevLog.File($"[DEBUG] PeakTextPool: EnsureTemplateAsync attempt {attempt + 1} - MenuAPI.CreateText returned null");
				}
			}
			catch (System.Exception ex)
			{
				DevLog.File($"[DEBUG] PeakTextPool: Exception in EnsureTemplateAsync attempt {attempt + 1}: {ex.GetType().Name}: {ex.Message}");
			}

			// Wait a short time before retrying to give MenuAPI/templating a chance to become ready.
			await UniTask.Delay(delayMs);
		}
		DevLog.File($"[DEBUG] PeakTextPool: EnsureTemplateAsync failed after {maxAttempts} attempts");
		return false;
	}

	// Get a PeakText instance. Try pool first, then instantiate template, then fallback to MenuAPI.CreateText
	public PeakText Get(string text)
	{
		PeakText instance = null;
		if (pool.Count > 0)
		{
			instance = pool.Dequeue();
			try { instance.gameObject.SetActive(true); } catch { }
			try { instance.transform.SetParent(poolContainerTransform, false); } catch { }
			try { instance.SetText(text); } catch { }
			// Ensure TMP settings are applied when reusing, but only once per instance
			try { if (!appliedSettings.Contains(instance.GetInstanceID())) { ApplyTMPSettings(instance); appliedSettings.Add(instance.GetInstanceID()); } } catch { }
			return instance;
		}

		if (template != null)
		{
			try
			{
				var go = GameObject.Instantiate(template.gameObject, poolContainerTransform, false);
				var pt = go.GetComponent<PeakText>();
				if (pt != null)
				{
					pt.SetText(text);
					// Apply TMP perf settings once for this instance
					try { if (!appliedSettings.Contains(pt.GetInstanceID())) { ApplyTMPSettings(pt); appliedSettings.Add(pt.GetInstanceID()); } } catch { }
					go.SetActive(true);
					DevLog.File($"[DEBUG] PeakTextPool: Instantiated new instance from template. poolSize={pool.Count}/{maxPoolSize}");
					return pt;
				}
			}
			catch { }
		}

		DevLog.File("[WARNING] PeakTextPool: Pool empty and no template, finally falling back！");
		// As a final fallback, create a very small GameObject and attach PeakText component
		var fallbackGO = new GameObject("UI_PeakText_Fallback", typeof(PeakText));
		var fallbackPT = fallbackGO.GetComponent<PeakText>();
		try { fallbackPT.SetText(text); } catch { }
		try { fallbackPT.transform.SetParent(poolContainerTransform, false); } catch { }
		try { if (!appliedSettings.Contains(fallbackPT.GetInstanceID())) { ApplyTMPSettings(fallbackPT); appliedSettings.Add(fallbackPT.GetInstanceID()); } } catch { }
		return fallbackPT;
	}

	// Apply TextMeshPro related settings to help avoid expensive auto-layout and autosizing
	private void ApplyTMPSettings(PeakText peakText)
	{
		if (peakText == null) return;
		try
		{
			var tm = peakText.TextMesh;
			if (tm == null) return;

			// Determine font size: prefer plugin config if available, otherwise keep existing
			float fontSize = 0f;
			try { fontSize = PeakChatOpsPlugin.FontSize.Value < 0 ? 1000000000 : PeakChatOpsPlugin.FontSize.Value; } catch { fontSize = tm.fontSize; }

			// Disable auto-sizing which can be expensive for frequent updates
			try { tm.enableAutoSizing = false; } catch { }

			// Set word wrapping mode to Wrap for multiline chat (try new API, fallback to legacy)
			try
			{
				// Use cached reflection PropertyInfo if available to reduce allocations
				var prop = textWrappingModeProp ?? (textWrappingModeProp = tm.GetType().GetProperty("textWrappingMode"));
				if (prop != null)
				{
					var enumType = prop.PropertyType;
					var wrapValue = System.Enum.Parse(enumType, "Wrap");
					prop.SetValue(tm, wrapValue);
				}
					else
					{
						// Fallback to legacy property via reflection to avoid compile-time deprecated reference
						var legacyProp = enableWordWrappingProp ?? (enableWordWrappingProp = tm.GetType().GetProperty("enableWordWrapping"));
						if (legacyProp != null)
						{
							try { legacyProp.SetValue(tm, true); } catch { }
						}
					}
			}
			catch
			{
					try
					{
						var legacyProp = enableWordWrappingProp ?? (enableWordWrappingProp = tm.GetType().GetProperty("enableWordWrapping"));
						if (legacyProp != null)
						{
							try { legacyProp.SetValue(tm, true); } catch { }
						}
					}
					catch { }
			}

			// Keep rich text enabled if messages use styling
			try { tm.richText = true; } catch { }

			// Use truncate to avoid costly overflow handling; change as needed
			try { tm.overflowMode = TMPro.TextOverflowModes.Truncate; } catch { }

			// Apply configured font size if valid
			try { if (fontSize > 0 && !float.IsInfinity(fontSize)) tm.fontSize = fontSize; } catch { }
		}
		catch { }
	}

	// Release the instance back to pool: deactivate and parent to idle container
	public void Release(PeakText instance)
	{
		if (instance == null) return;
		try
		{
			instance.transform.SetParent(poolContainerTransform, false);
			instance.gameObject.SetActive(false);
			if (pool.Count < maxPoolSize)
			{
				pool.Enqueue(instance);
				DevLog.File($"[DEBUG] PeakTextPool: Released instance to pool. poolSize={pool.Count}/{maxPoolSize}");
			}
			else
			{
				// Pool full: destroy excess to avoid unbounded memory growth
				// Remove tracking since instance will be destroyed
				try { appliedSettings.Remove(instance.GetInstanceID()); } catch { }
				GameObject.Destroy(instance.gameObject);
				DevLog.File($"[DEBUG] PeakTextPool: Pool full, destroyed released instance. poolSize={pool.Count}/{maxPoolSize}");
			}
		}
		catch { }
	}

	// Pre-create N instances to warm the pool. Uses template or MenuAPI.CreateText.
	public void Warmup(int count)
	{
		if (count <= 0) return;
		for (int i = 0; i < count; i++)
		{
			var pt = Get("");
			// Immediately release to return to pool
			Release(pt);
		}
	}

	// Remove all pooled instances (destroy) and reset pool size to zero
	public void Clear()
	{
		while (pool.Count > 0)
		{
			var pt = pool.Dequeue();
			try { appliedSettings.Remove(pt.GetInstanceID()); } catch { }
			try { GameObject.Destroy(pt.gameObject); } catch { }
		}
		// Destroy the container as well
		try { GameObject.Destroy(poolContainerTransform.gameObject); } catch { }
		DevLog.File("[DEBUG] PeakTextPool: Cleared pool and destroyed container");
	}

	// Trim pool to target size by destroying excess instances
	public void Trim(int targetSize)
	{
		if (targetSize < 0) targetSize = 0;
		while (pool.Count > targetSize)
		{
			var pt = pool.Dequeue();
			try { appliedSettings.Remove(pt.GetInstanceID()); } catch { }
			try { GameObject.Destroy(pt.gameObject); } catch { }
		}
		DevLog.File($"[DEBUG] PeakTextPool: Trimmed pool to {targetSize}. current={pool.Count}");
	}
}
