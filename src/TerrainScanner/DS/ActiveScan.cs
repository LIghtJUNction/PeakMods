using UnityEngine;
namespace TerrainScanner;
public class ActiveScan : MonoBehaviour
{
    public static KeyCode activeKey = KeyCode.Q;
    
    // Start is called before the first frame update
    void Start()
    {

    }
    

    
    // Update is called once per frame
    void Update()
    {

        if( Input.GetKeyDown( activeKey ) ) {
            // 使用摄像机的 transform（在第一人称游戏中这就是玩家视角）
            ScanFeature.ExecuteScan( transform );
            TerrainScannerPlugin.Logger.LogInfo( "TerrainScanner : scan executed" );
        }
    }
}
