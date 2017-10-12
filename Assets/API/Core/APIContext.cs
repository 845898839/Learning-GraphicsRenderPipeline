public class APIContext {

    private static APIFrameBufferManager _FrameBufferManager;
    private static APIPipeline _Pipeline;
    private static APIDataBuffer _DataBuffer;
    private static APIViewport _Viewport;
    private static APIMonitor _Monitor;
    private static APIStates _States;

    public static APIFrameBufferManager frameBufferManager { get { return _FrameBufferManager; } }
    public static APIViewport viewport { get { return _Viewport; } }
    public static APIPipeline pipeline { get { return _Pipeline; } }
    public static APIDataBuffer dataBuffer { get { return _DataBuffer; } }
    public static APIMonitor monitor { get { return _Monitor; } }
    public static APIStates states { get { return _States; } }

    public APIContext(int width, int height) {
        _FrameBufferManager = new APIFrameBufferManager(width, height);
        _Pipeline = new APIPipeline();
        _DataBuffer = new APIDataBuffer();
        _Viewport = new APIViewport();
        _Monitor = new APIMonitor();
        _States = new APIStates();
    }
}
