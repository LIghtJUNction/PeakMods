using UnityEngine;
namespace TerrainScanner;
public class ActiveScan : MonoBehaviour
{
    public static KeyCode activeKey;
    // Start is called before the first frame update
    void Start()
    {
        activeKey = TerrainScannerPlugin.Instance.configActivationKey.Value;
    }
    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown( activeKey ) ) {
            ScanFeature.ExecuteScan( transform );
            TerrainScannerPlugin.Logger.LogInfo( "TerrainScanner : scan" );
        }
    }
}
