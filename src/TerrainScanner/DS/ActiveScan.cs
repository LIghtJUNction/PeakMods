using UnityEngine;
namespace TerrainScanner;
public class ActiveScan : MonoBehaviour
{
    public static KeyCode activeKey;
    private bool isInitialized = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // 延迟初始化，确保 Instance 已经设置
        TryInitialize();
    }
    
    private void TryInitialize()
    {
        if (isInitialized) return;
        
        if (TerrainScannerPlugin.Instance != null)
        {
            activeKey = TerrainScannerPlugin.Instance.configActivationKey.Value;
            isInitialized = true;
            TerrainScannerPlugin.Logger.LogInfo("[ActiveScan] Initialized successfully");
        }
        else
        {
            TerrainScannerPlugin.Logger?.LogWarning("[ActiveScan] TerrainScannerPlugin.Instance is null, retrying...");
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // 如果还未初始化，尝试初始化
        if (!isInitialized)
        {
            TryInitialize();
            return;
        }
        
        if( Input.GetKeyDown( activeKey ) ) {
            ScanFeature.ExecuteScan( transform );
            TerrainScannerPlugin.Logger.LogInfo( "TerrainScanner : scan" );
        }
    }
}
