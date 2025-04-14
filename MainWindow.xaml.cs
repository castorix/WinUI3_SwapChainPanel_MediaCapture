using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

using GlobalStructures;
using static GlobalStructures.GlobalTools;
using Direct2D;
using static Direct2D.D2DTools;
using DXGI;
using static DXGI.DXGITools;
using WIC;
using static WIC.WICTools;
using D3D11;
using Windows.Devices.Enumeration;
using Windows.Media.Devices;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Graphics.Imaging;
using WinRT;
using System.Threading;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Media.MediaProperties;
using Windows.Media;
using Windows.Media.Effects;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.WindowsAppSDK.Runtime.Packages;
using System.Text;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Management.Deployment;
using Microsoft.WindowsAppSDK.Runtime;
using Windows.ApplicationModel;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using VideoEffectComponent;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Devices.I2c;
using ABI.Windows.Foundation;


// References :
// https://github.com/microsoft/windows-universal-samples/tree/main/Samples/CameraFrames
// https://github.com/microsoft/Windows-Camera/tree/1b890286ce6a1e61edd3b92e7027353b0ac6c926/Samples/MediaCaptureWinUI3/MediaCaptureWinUI3

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3_SwapChainPanel_MediaCapture
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        [ComImport]
        [Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDirect3DDxgiInterfaceAccess
        {
            HRESULT GetInterface([MarshalAs(UnmanagedType.LPStruct)] Guid iid, out IntPtr ppv);
        }

        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IMemoryBufferByteAccess
        {
            void GetBuffer(out IntPtr buffer, out uint capacity);
        }

        ID2D1Factory m_pD2DFactory = null;
        ID2D1Factory1 m_pD2DFactory1 = null;
        IWICImagingFactory m_pWICImagingFactory = null;
        IWICImagingFactory2 m_pWICImagingFactory2 = null;

        IntPtr m_pD3D11DevicePtr = IntPtr.Zero; // Used in CreateSwapChain
        Direct2D.ID3D11DeviceContext m_pD3D11DeviceContext = null; // Released in Clean : not used
        IDXGIDevice1 m_pDXGIDevice = null;
        ID2D1DeviceContext m_pD2DDeviceContext = null;
        ID2D1DeviceContext5 m_pD2DDeviceContext5 = null;

        IDXGISwapChain1 m_pDXGISwapChain1 = null;
        ID2D1Bitmap1 m_pD2DTargetBitmap = null;

        ID2D1SolidColorBrush m_pD2DMainBrush = null;
        ID2D1SolidColorBrush m_pD2DSolidColorBrushPink = null;
        ID2D1Bitmap1 m_pD2DBitmap1 = null;


        private IntPtr hWndMain = IntPtr.Zero;
        private Microsoft.UI.Windowing.AppWindow _apw;

        private DeviceInformationCollection m_deviceList;
        System.Collections.ObjectModel.ObservableCollection<ComboBoxItem> devices = new System.Collections.ObjectModel.ObservableCollection<ComboBoxItem>();
        private MediaCapture m_MediaCapture = null;
        private MediaFrameSource m_frameSource = null;
        private MediaFrameFormat m_currentFrameFormat = null;
        private MediaFrameReader m_FrameReader = null;
        private bool m_bPreviewing = false;
        private bool m_bRecording = false;
        MediaMirroringOptions m_Mirror = MediaMirroringOptions.None;
        MediaRotation m_Rotation = MediaRotation.None;

        public ObservableCollection<MediaFrameFormatWrapper> MediaFormats { get; set; } = new ObservableCollection<MediaFrameFormatWrapper>();
        public Windows.Media.Playback.MediaPlayer m_MP = new Windows.Media.Playback.MediaPlayer();

        public MainWindow()
        {
            this.InitializeComponent();

            hWndMain = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWndMain);
            _apw = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);
            _apw.MoveAndResize(new Windows.Graphics.RectInt32(400, 200, 1100, 800));
            this.Title = "WinUI 3 : MediaCapture with SwapChainPanel";

            Application.Current.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(Microsoft.UI.Colors.LightSteelBlue);
            Application.Current.Resources["ButtonBackgroundPressed"] = new SolidColorBrush(Microsoft.UI.Colors.MidnightBlue);

            m_pWICImagingFactory = (IWICImagingFactory)Activator.CreateInstance(Type.GetTypeFromCLSID(WICTools.CLSID_WICImagingFactory));
            m_pWICImagingFactory2 = (IWICImagingFactory2)m_pWICImagingFactory;
            HRESULT hr = CreateD2D1Factory();
            if (hr == HRESULT.S_OK)
            {
                hr = CreateDeviceContext();
                hr = CreateDeviceResources();
                hr = CreateSwapChain(IntPtr.Zero);
                if (hr == HRESULT.S_OK)
                {
                    hr = ConfigureSwapChain(hWndMain);
                    ISwapChainPanelNative panelNative = WinRT.CastExtensions.As<ISwapChainPanelNative>(scp1);
                    hr = panelNative.SetSwapChain(m_pDXGISwapChain1);
                    scp1.SizeChanged += scp1_SizeChanged;
                    CompositionTarget.Rendering += CompositionTarget_Rendering;
                }
            }
            this.Closed += MainWindow_Closed;

            FillDevices();

            LoadOverlayImage();
            ChangeCursor(imgOverlay, Microsoft.UI.Input.InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.Hand));
            //imgOverlay.Tapped += ImgOverlay_Tapped;
            ChangeCursor(rectVignetteColor, Microsoft.UI.Input.InputSystemCursor.Create(Microsoft.UI.Input.InputSystemCursorShape.Hand));

            LoadMP3("Assets\\Camera_Click.mp3");
        }

        private async void LoadMP3(string sRelativePath)
        {
            string sAbsolutePath = Path.Combine(AppContext.BaseDirectory, sRelativePath);
            StorageFile sfFile = await StorageFile.GetFileFromPathAsync(sAbsolutePath);
            m_MP.Source = Windows.Media.Core.MediaSource.CreateFromStorageFile(sfFile);
        }

        private void ChangeCursor(UIElement control, Microsoft.UI.Input.InputCursor cursor)
        {
            var cursorProperty = typeof(UIElement).GetProperty("ProtectedCursor", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var currentCursor = cursorProperty?.GetValue(imgOverlay) as InputSystemCursor;
            Microsoft.UI.Input.InputCursor ic = cursor;
            var methodInfo = typeof(UIElement).GetMethod("set_ProtectedCursor", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (methodInfo != null)
            {
                methodInfo.Invoke(control, new object[] { ic });
            }
            //ic.Dispose();
        }

        private async void ImgOverlay_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var fop = new Windows.Storage.Pickers.FileOpenPicker();
            WinRT.Interop.InitializeWithWindow.Initialize(fop, hWndMain);
            fop.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            var types = new List<string> { ".jpg", ".png", ".gif", ".bmp", ".tif" };
            foreach (var type in types)
                fop.FileTypeFilter.Add(type);
           
            var file = await fop.PickSingleFileAsync();
            if (file != null)
            {
                using (var stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    //m_OverlayImage?.Dispose();
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    m_OverlayImage = await decoder.GetSoftwareBitmapAsync();
                    if (m_OverlayImage.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || m_OverlayImage.BitmapAlphaMode == BitmapAlphaMode.Straight)
                    {
                        m_OverlayImage = SoftwareBitmap.Convert(m_OverlayImage, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    }
                    var source = new SoftwareBitmapSource();
                    await source.SetBitmapAsync(m_OverlayImage);
                    imgOverlay.Source = source;
                }
            }
        }

        private SoftwareBitmap m_OverlayImage, m_OverlayImageScaled = null;

        private async void LoadOverlayImage()
        {
            string sExePath = AppContext.BaseDirectory;
            string sImagePath = System.IO.Path.Combine(sExePath, "Assets\\Butterfly_Blue_126x100.png");
            StorageFile file = await StorageFile.GetFileFromPathAsync(sImagePath);

            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);
                m_OverlayImage = await decoder.GetSoftwareBitmapAsync();
                if (m_OverlayImage.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || m_OverlayImage.BitmapAlphaMode == BitmapAlphaMode.Straight)
                {
                    m_OverlayImage = SoftwareBitmap.Convert(m_OverlayImage, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }
                var source = new SoftwareBitmapSource();
                await source.SetBitmapAsync(m_OverlayImage);
                imgOverlay.Source = source;
            }
        }
        
        bool m_bCameraRender = false;
        private void CompositionTarget_Rendering(object sender, object e)
        {
            HRESULT hr = HRESULT.S_OK;
            hr = Render();
        }

        HRESULT Render()
        {
            HRESULT hr = HRESULT.S_OK;
            if (m_pD2DDeviceContext != null)
            {
                m_pD2DDeviceContext.BeginDraw();
                
                //m_pD2DDeviceContext.Clear(new ColorF(ColorF.Enum.Orange, 1.0f));
                m_pD2DDeviceContext.Clear(new ColorF(ColorF.Enum.Black, 1.0f));
                //m_pD2DDeviceContext.Clear(null);               

                D2D1_SIZE_F size = m_pD2DDeviceContext.GetSize();              

                if (m_pD2DBitmap1 != null)
                {
                    D2D1_SIZE_F sizeBmpBackground = m_pD2DBitmap1.GetSize();
                    
                    float renderTargetAspect = size.width / size.height;
                    float bitmapAspect = sizeBmpBackground.width / sizeBmpBackground.height;
                   
                    D2D1_RECT_F destRectBackground;
                    if (bitmapAspect > renderTargetAspect)
                    {
                        // Bitmap is wider than the render target
                        float scaledHeight = size.width / bitmapAspect;
                        float verticalOffset = (size.height - scaledHeight) / 2.0f;
                        destRectBackground = new D2D1_RECT_F(0.0f, verticalOffset, size.width, verticalOffset + scaledHeight);
                    }
                    else
                    {
                        // Bitmap is taller than the render target
                        float scaledWidth = size.height * bitmapAspect;
                        float horizontalOffset = (size.width - scaledWidth) / 2.0f;
                        destRectBackground = new D2D1_RECT_F(horizontalOffset, 0.0f, horizontalOffset + scaledWidth, size.height);
                    }
                    
                    D2D1_RECT_F sourceRectBackground = new D2D1_RECT_F(0.0f, 0.0f, sizeBmpBackground.width, sizeBmpBackground.height);

                    if (!m_bCameraRender)
                    {
                        m_pD2DDeviceContext.SetTransform(Matrix3x2F.Identity());
                        m_pD2DDeviceContext.DrawBitmap(m_pD2DBitmap1, ref destRectBackground, 1.0f, D2D1_BITMAP_INTERPOLATION_MODE.D2D1_BITMAP_INTERPOLATION_MODE_LINEAR, ref sourceRectBackground);
                    }
                    else
                    {                        
                        if (m_pD3D11DevicePtr != IntPtr.Zero && m_SharedHandle != IntPtr.Zero)
                        {
                            // If Effect and resizing
                            // D3D11 ERROR: ID3D11Device::OpenSharedResource: Returning E_INVALIDARG, meaning invalid parameters were passed. [ STATE_CREATION ERROR #381: DEVICE_OPEN_SHARED_RESOURCE_INVALIDARG_RETURN]
                            ID3D11Device pD3D11Device = Marshal.GetObjectForIUnknown(m_pD3D11DevicePtr) as ID3D11Device;
                            hr = pD3D11Device.OpenSharedResource(m_SharedHandle, typeof(D3D11.ID3D11Texture2D).GUID, out IntPtr texturePtr);
                            if (hr == HRESULT.S_OK)
                            {
                                D3D11.ID3D11Texture2D pSharedTextureMainThread = Marshal.GetObjectForIUnknown(texturePtr) as D3D11.ID3D11Texture2D;
                                IDXGISurface pDXGISurface = pSharedTextureMainThread as IDXGISurface;

                                D2D1_BITMAP_PROPERTIES1 bitmapProperties = new D2D1_BITMAP_PROPERTIES1();
                                bitmapProperties.bitmapOptions = D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_NONE;// D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_TARGET | D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_CANNOT_DRAW;
                                //bitmapProperties.pixelFormat = D2DTools.PixelFormat(DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_IGNORE);
                                bitmapProperties.pixelFormat = D2DTools.PixelFormat(DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_PREMULTIPLIED);
                                //uint nDPI = GetDpiForWindow(hWndMain);
                                //bitmapProperties.dpiX = nDPI;
                                //bitmapProperties.dpiY = nDPI;
                                bitmapProperties.dpiX = 96.0f;
                                bitmapProperties.dpiY = 96.0f;

                                //ID2D1ColorContext1 pD2D1ColorContext1 = null;
                                //hr = m_pD2DDeviceContext5.CreateColorContextFromDxgiColorSpace(DXGI_COLOR_SPACE_TYPE.DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709, out pD2D1ColorContext1);
                                //if (hr == HRESULT.S_OK && pD2D1ColorContext1 != null)
                                //    bitmapProperties.colorContext = pD2D1ColorContext1;                               

                                ID2D1Bitmap1 pD2DBitmap1;
                                hr = m_pD2DDeviceContext.CreateBitmapFromDxgiSurface(pDXGISurface, bitmapProperties, out pD2DBitmap1);
                                if (hr == HRESULT.S_OK && pD2DBitmap1 != null)
                                {
                                    sizeBmpBackground = pD2DBitmap1.GetSize();

                                    renderTargetAspect = size.width / size.height;
                                    bitmapAspect = sizeBmpBackground.width / sizeBmpBackground.height;

                                    if (bitmapAspect > renderTargetAspect)
                                    {
                                        // Bitmap is wider than the render target
                                        float scaledHeight = size.width / bitmapAspect;
                                        float verticalOffset = (size.height - scaledHeight) / 2.0f;
                                        destRectBackground = new D2D1_RECT_F(0.0f, verticalOffset, size.width, verticalOffset + scaledHeight);
                                    }
                                    else
                                    {
                                        // Bitmap is taller than the render target
                                        float scaledWidth = size.height * bitmapAspect;
                                        float horizontalOffset = (size.width - scaledWidth) / 2.0f;
                                        destRectBackground = new D2D1_RECT_F(horizontalOffset, 0.0f, horizontalOffset + scaledWidth, size.height);
                                    }

                                    sourceRectBackground = new D2D1_RECT_F(0.0f, 0.0f, sizeBmpBackground.width, sizeBmpBackground.height);

                                    //pD2DBitmap1.GetColorContext(colorContext: out ID2D1ColorContext pD2D1ColorContext);
                                    //D2D1_COLOR_SPACE cs = pD2D1ColorContext.GetColorSpace();

                                    // If no custom effect, Brightness is too high with VideoProcessor : Color space or gamma mismatch ?
                                    //if (!(cbOverlayImage.IsChecked == true || tsMirror.IsOn || cbGrayscale.IsChecked == true ||
                                    // cbRGB.IsChecked == true || cbInvert.IsChecked == true || cbBrightness.IsChecked == true ||
                                    // cbEmboss.IsChecked == true || cbEdgeDetection.IsChecked == true || cbGaussianBlur.IsChecked == true ||
                                    // cbSharpen.IsChecked == true || cbVignette.IsChecked == true) || (tsRotation.IsOn || cbFaceDetection.IsChecked == true))
                                    //{
                                    //    ID2D1ColorContext1 pD2DColorContextSource = null;
                                    //    hr = m_pD2DDeviceContext5.CreateColorContextFromDxgiColorSpace(DXGI_COLOR_SPACE_TYPE.DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709, out pD2DColorContextSource);
                                    //    IntPtr pD2DColorContextSourcePtr = Marshal.GetComInterfaceForObject(pD2DColorContextSource, typeof(ID2D1ColorContext1));

                                    //    ID2D1ColorContext1 pD2DColorContextDest = null;
                                    //    //hr = m_pD2DDeviceContext.CreateColorContext(D2D1_COLOR_SPACE., IntPtr.Zero, 0, out pD2DColorContextDest);
                                    //    hr = m_pD2DDeviceContext5.CreateColorContextFromDxgiColorSpace(DXGI_COLOR_SPACE_TYPE.DXGI_COLOR_SPACE_RGB_FULL_G10_NONE_P709, out pD2DColorContextDest);
                                    //    IntPtr pD2DColorContextDestPtr = Marshal.GetComInterfaceForObject(pD2DColorContextDest, typeof(ID2D1ColorContext));

                                    //    ID2D1Effect pEffect = null;
                                    //    hr = m_pD2DDeviceContext.CreateEffect(CLSID_D2D1ColorManagement, out pEffect);
                                    //    pEffect.SetInput(0, pD2DBitmap1);
                                    //    SetEffectIntPtr(pEffect, (uint)D2D1_COLORMANAGEMENT_PROP.D2D1_COLORMANAGEMENT_PROP_SOURCE_COLOR_CONTEXT, pD2DColorContextSourcePtr);
                                    //    SetEffectIntPtr(pEffect, (uint)D2D1_COLORMANAGEMENT_PROP.D2D1_COLORMANAGEMENT_PROP_DESTINATION_COLOR_CONTEXT, pD2DColorContextDestPtr);

                                    //    SetEffectInt(pEffect, (uint)D2D1_COLORMANAGEMENT_PROP.D2D1_COLORMANAGEMENT_PROP_QUALITY, (uint)D2D1_COLORMANAGEMENT_QUALITY.D2D1_COLORMANAGEMENT_QUALITY_BEST);

                                    //    float scaleX = (destRectBackground.right - destRectBackground.left) / sizeBmpBackground.width;
                                    //    float scaleY = (destRectBackground.bottom - destRectBackground.top) / sizeBmpBackground.height;
                                    //    float offsetX = destRectBackground.left;
                                    //    float offsetY = destRectBackground.top;
                                    //    var transform = Matrix3x2F.Scale(scaleX, scaleY) * Matrix3x2F.Translation(offsetX, offsetY);
                                    //    m_pD2DDeviceContext.SetTransform(transform);

                                    //    ID2D1Image pOutputImage = null;
                                    //    pEffect.GetOutput(out pOutputImage);
                                    //    D2D1_POINT_2F pt = new D2D1_POINT_2F(0, 0);
                                    //    D2D1_RECT_F sourceRectangle = new D2D1_RECT_F(0, 0, size.width, size.height);
                                    //    m_pD2DDeviceContext.DrawImage(pOutputImage, ref pt, ref sourceRectBackground, D2D1_INTERPOLATION_MODE.D2D1_INTERPOLATION_MODE_LINEAR, D2D1_COMPOSITE_MODE.D2D1_COMPOSITE_MODE_SOURCE_OVER);

                                    //    SafeRelease(ref pD2DColorContextSource);
                                    //    Marshal.Release(pD2DColorContextSourcePtr);
                                    //    SafeRelease(ref pD2DColorContextDest);
                                    //    Marshal.Release(pD2DColorContextDestPtr);
                                    //    SafeRelease(ref pEffect);
                                    //    SafeRelease(ref pOutputImage);
                                    //}
                                    //else
                                    //{
                                    //    m_pD2DDeviceContext.DrawBitmap(pD2DBitmap1, ref destRectBackground, 1.0f, D2D1_BITMAP_INTERPOLATION_MODE.D2D1_BITMAP_INTERPOLATION_MODE_LINEAR, ref sourceRectBackground);
                                    //}

                                    m_pD2DDeviceContext.DrawBitmap(pD2DBitmap1, ref destRectBackground, 1.0f, D2D1_BITMAP_INTERPOLATION_MODE.D2D1_BITMAP_INTERPOLATION_MODE_LINEAR, ref sourceRectBackground);

                                    if (m_DetectedFaces.Count > 0)
                                    {                               
                                        float nScaleX = (destRectBackground.right - destRectBackground.left) / sizeBmpBackground.width;
                                        float nScaleY = (destRectBackground.bottom - destRectBackground.top) / sizeBmpBackground.height;
                                        float nOffsetX = destRectBackground.left;
                                        float nOffsetY = destRectBackground.top;
                                        // "Collection was modified; enumeration operation may not execute"
                                        //foreach (var faceBounds in m_DetectedFaces)
                                        for (int i = 0; i < m_DetectedFaces.Count; i++)
                                        {
                                            var faceBounds = m_DetectedFaces[i];
                                            float nFaceX = faceBounds.X * nScaleX + nOffsetX;
                                            float nFaceY = faceBounds.Y * nScaleY + nOffsetY;
                                            float nFaceWidth = faceBounds.Width * nScaleX;
                                            float nFaceHeight = faceBounds.Height * nScaleY;
                                            var rect = RectF(nFaceX, nFaceY, nFaceX + nFaceWidth, nFaceY + nFaceHeight);
                                            m_pD2DDeviceContext.DrawRectangle(rect, m_pD2DSolidColorBrushPink, 2.0f);
                                        }                                       
                                    }
                                    SafeRelease(ref pD2DBitmap1);
                                    SafeRelease(ref bitmapProperties.colorContext);
                                }
                                SafeRelease(ref pSharedTextureMainThread);
                                Marshal.Release(texturePtr);                                

                                //SafeRelease(ref pD2D1ColorContext1);
                               
                                SafeRelease(ref pDXGISurface);
                            }
                            else
                            {                               
                                //Debug.WriteLine($"m_SharedHandle: {m_SharedHandle}");
                            }
                            SafeRelease(ref pD3D11Device);
                        }                                              
                    }
                }              

                // For test
                // m_pD2DDeviceContext.FillEllipse(Ellipse(new Direct2D.D2D1_POINT_2F(300, 300), 100.0f, 100.0f), m_pD2DMainBrush);

                hr = m_pD2DDeviceContext.EndDraw(out ulong tag1, out ulong tag2);

                if ((uint)hr == D2DTools.D2DERR_RECREATE_TARGET)
                {
                    m_pD2DDeviceContext.SetTarget(null);
                    SafeRelease(ref m_pD2DDeviceContext);
                    hr = CreateDeviceContext();
                    CleanDeviceResources();
                    hr = CreateDeviceResources();
                    hr = CreateSwapChain(IntPtr.Zero);
                    hr = ConfigureSwapChain(hWndMain);
                }
                hr = m_pDXGISwapChain1.Present(1, 0);
            }
            return (hr);
        }

        private async void FillDevices()
        {
            cmbDevices.Items.Clear();
            m_deviceList = await DeviceInformation.FindAllAsync(MediaDevice.GetVideoCaptureSelector());
            foreach (var device in m_deviceList)
            {
                devices.Add(new ComboBoxItem() { Content = device.Name });
            }
        }

        private void cbGrayscale_Checked(object sender, RoutedEventArgs e)
        {
            if (cbRGB.IsChecked == true)
            {
                cbRGB.IsChecked = false;
            }
            if (cbEmboss.IsChecked == true)
            {
                cbEmboss.IsChecked = false;
            }
            if (cbEdgeDetection.IsChecked == true)
            {
                cbEdgeDetection.IsChecked = false;
            }
        }

        private void cbRGB_Checked(object sender, RoutedEventArgs e)
        {
            if (cbGrayscale.IsChecked == true)
            {
                cbGrayscale.IsChecked = false;
            }
            if (cbEmboss.IsChecked == true)
            {
                cbEmboss.IsChecked = false;
            }
            if (cbEdgeDetection.IsChecked == true)
            {
                cbEdgeDetection.IsChecked = false;
            }
        }

        private void cbEmboss_Checked(object sender, RoutedEventArgs e)
        {
            if (cbRGB.IsChecked == true)
            {
                cbRGB.IsChecked = false;
            }
            if (cbGrayscale.IsChecked == true)
            {
                cbGrayscale.IsChecked = false;
            }
            if (cbEdgeDetection.IsChecked == true)
            {
                cbEdgeDetection.IsChecked = false;
            } 
            if (cbGaussianBlur.IsChecked == true)
            {
                cbGaussianBlur.IsChecked = false;
            }
            if (cbSharpen.IsChecked == true)
            {
                cbSharpen.IsChecked = false;
            }
            if (cbVignette.IsChecked == true)
            {
                cbVignette.IsChecked = false;
            }
            if (cbFaceDetection.IsChecked == true)
            {
                cbFaceDetection.IsChecked = false;
            }
        }

        private void cbGaussianBlur_Checked(object sender, RoutedEventArgs e)
        {
            if (cbEdgeDetection.IsChecked == true)
            {
                cbEdgeDetection.IsChecked = false;
            }
            if (cbEmboss.IsChecked == true)
            {
                cbEmboss.IsChecked = false;
            }
            if (cbSharpen.IsChecked == true)
            {
                cbSharpen.IsChecked = false;
            }
            if (cbVignette.IsChecked == true)
            {
                cbVignette.IsChecked = false;
            }
            if (cbFaceDetection.IsChecked == true)
            {
                cbFaceDetection.IsChecked = false;
            }
        }

        private void cbSharpen_Checked(object sender, RoutedEventArgs e)
        {
            if (cbEdgeDetection.IsChecked == true)
            {
                cbEdgeDetection.IsChecked = false;
            }
            if (cbEmboss.IsChecked == true)
            {
                cbEmboss.IsChecked = false;
            }  
            if (cbGaussianBlur.IsChecked == true)
            {
                cbGaussianBlur.IsChecked = false;
            }
            if (cbVignette.IsChecked == true)
            {
                cbVignette.IsChecked = false;
            }
            //if (cbFaceDetection.IsChecked == true)
            //{
            //    cbFaceDetection.IsChecked = false;
            //}
        }          

        private void cbEdgeDetection_Checked(object sender, RoutedEventArgs e)
        {
            if (cbRGB.IsChecked == true)
            {
                cbRGB.IsChecked = false;
            }
            if (cbGrayscale.IsChecked == true)
            {
                cbGrayscale.IsChecked = false;
            }
            if (cbEmboss.IsChecked == true)
            {
                cbEmboss.IsChecked = false;
            }          
            if (cbGaussianBlur.IsChecked == true)
            {
                cbGaussianBlur.IsChecked = false;
            }
            if (cbSharpen.IsChecked == true)
            {
                cbSharpen.IsChecked = false;
            }   
            if (cbVignette.IsChecked == true)
            {
                cbVignette.IsChecked = false;
            }
            if (cbFaceDetection.IsChecked == true)
            {
                cbFaceDetection.IsChecked = false;
            }
        }

        private void cbVignette_Checked(object sender, RoutedEventArgs e)
        {
            if (cbEdgeDetection.IsChecked == true)
            {
                cbEdgeDetection.IsChecked = false;
            }
            if (cbEmboss.IsChecked == true)
            {
                cbEmboss.IsChecked = false;
            }
            if (cbGaussianBlur.IsChecked == true)
            {
                cbGaussianBlur.IsChecked = false;
            }
            if (cbSharpen.IsChecked == true)
            {
                cbSharpen.IsChecked = false;
            }
        }

        private void cbFaceDetection_Checked(object sender, RoutedEventArgs e)
        {
            if (cbEmboss.IsChecked == true)
            {
                cbEmboss.IsChecked = false;
            }
            if (cbEdgeDetection.IsChecked == true)
            {
                cbEdgeDetection.IsChecked = false;
            }
            if (cbGaussianBlur.IsChecked == true)
            {
                cbGaussianBlur.IsChecked = false;
            }
        }

        int m_Red = 0, m_Green = 0, m_Blue = 0;
        float m_StrengthEmboss = 5.0f;
        float m_StrengthEdgeDetection = 0.5f;
        float m_DeviationGaussianBlur = 3.0f;
        float m_SharpnessSharpen = 0.5f;

        private void sliderR_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider s = sender as Slider;
            m_Red = (int)s.Value;
        } 

        private void sliderG_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {  
            Slider s = sender as Slider;
            m_Green = (int)s.Value;
        }

        private void sliderB_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider s = sender as Slider;
            m_Blue = (int)s.Value;
        }

        private void sliderStrengthEmboss_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider s = sender as Slider;
            m_StrengthEmboss = (float)s.Value;
        }

        private void sliderStandardDeviationGaussianBlur_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider s = sender as Slider;
            m_DeviationGaussianBlur = (float)s.Value;
        }

        private void sliderSharpnessSharpen_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider s = sender as Slider;
            m_SharpnessSharpen = (float)s.Value;
        }

        private void sliderStrengthEdgeDetection_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider s = sender as Slider;
            m_StrengthEdgeDetection = (float)s.Value;
        }

        private async void rectVignetteColor_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var picker = new ColorPicker
            {
                IsAlphaEnabled = true,
                Color = ((SolidColorBrush)rectVignetteColor.Fill).Color
            };

            var dialog = new ContentDialog
            {
                Title = "Choose a Color",
                Content = picker,
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                rectVignetteColor.Fill = new SolidColorBrush(picker.Color);
            }
        }

        private void cmbFrameFormats_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            var comboBox = sender as Microsoft.UI.Xaml.Controls.ComboBox;
            if (comboBox.SelectedItem is MediaFrameFormatWrapper selectedFormat)
            {
                string formatDetails = selectedFormat.DisplayString;
                //int nWidth = selectedFormat.Width;
                //int nHeight = selectedFormat.Height;
                //int nNumerator = selectedFormat.FrameRateNumerator;
                //int nDenominator = selectedFormat.FrameRateDenominator;

                m_currentFrameFormat = selectedFormat.Format;
                
                //System.Diagnostics.Debug.WriteLine($"Selected Format: {formatDetails}");               
            }
        }

        public class MediaFrameFormatWrapper
        {
            public MediaFrameFormat Format { get; }

            public MediaFrameFormatWrapper(MediaFrameFormat format)
            {
                Format = format;
            }

            //public uint VideoFormatWidth { get; set; }
            //public uint VideoFormatHeight { get; set; }

            //public string DisplayString => $"{Format.Subtype}-{Format.VideoFormat.Width}*{Format.VideoFormat.Height}";
            public string DisplayString => string.Format("{0} | {1} | {2} x {3} | {4:#.##}fps",
                Format.MajorType,
                Format.Subtype,
                Format.VideoFormat?.Width,
                Format.VideoFormat?.Height,
                Math.Round((double)Format.FrameRate.Numerator / Format.FrameRate.Denominator, 2));
        }

        private async Task<bool> InitializeMediaCapture()
        {
            if (m_MediaCapture != null)
            {
                m_MediaCapture.Dispose();
                m_MediaCapture = null;
            }
            m_MediaCapture = new();          

            //var audioDeviceId = await GetAudioCaptureDevicesAsync();

            //var mfsg = (await MediaFrameSourceGroup.FindAllAsync())?.FirstOrDefault();
            var captureSettings = new MediaCaptureInitializationSettings
            {
                //AudioDeviceId = audioDeviceId, // Select the desired microphone
                //AudioDeviceId = string.Empty,
                VideoDeviceId = m_deviceList[cmbDevices.SelectedIndex].Id,// \\?\ROOT#IMAGE#0000#{e5323777-f976-4f5b-9b55-b94699c46e44}\global
                //SourceGroup = mfsg,
                StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                MediaCategory = MediaCategory.Media,
                AudioProcessing = AudioProcessing.Default,
                //AudioProcessing = AudioProcessing.Raw,
                // frame?.VideoMediaFrame =>
                MemoryPreference = MediaCaptureMemoryPreference.Auto // GPU => SoftwareBitmap = null, Direct3DSurface != null
                //MemoryPreference = MediaCaptureMemoryPreference.Cpu // SoftwareBitmap != null, Direct3DSurface = null                
            };
            
            //IReadOnlyList<MediaCaptureVideoProfile> profiles = MediaCapture.FindKnownVideoProfiles(captureSettings.VideoDeviceId, KnownVideoProfile.VideoRecording);
         
            await m_MediaCapture.InitializeAsync(captureSettings);
           
            //var videoDeviceController = m_MediaCapture.VideoDeviceController;
            //var supportedVideoFrameRates = videoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview);

            //var regionsControl = m_MediaCapture.VideoDeviceController.RegionsOfInterestControl;
            //bool bFaceDetectionFocusAndExposureSupported = regionsControl.MaxRegions > 0 &&
            //    (regionsControl.AutoExposureSupported || regionsControl.AutoFocusSupported);
            //if (bFaceDetectionFocusAndExposureSupported)
            //    cbFaceDetection.Visibility = Visibility.Visible;
            //else
            //    cbFaceDetection.Visibility = Visibility.Collapsed;

            //cbFaceDetection.Visibility = Visibility.Visible;

            return true;
        }

        private async void cmbDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbFrameFormats.IsEnabled = false;
            bool bStatus = await InitializeMediaCapture();
            if (bStatus)
            {
                MediaFormats.Clear();
                m_currentFrameFormat = null;
                //var frameSource = m_MediaCapture.FrameSources.Values.FirstOrDefault(source => source.Info.SourceKind == MediaFrameSourceKind.Color);

                //MediaFrameSource frameSource = null;
                MediaFrameSource previewSource = m_MediaCapture.FrameSources.FirstOrDefault(source => source.Value.Info.MediaStreamType == MediaStreamType.VideoPreview
                                                                                       && source.Value.Info.SourceKind == MediaFrameSourceKind.Color).Value;
                if (previewSource != null)
                {
                    m_frameSource = previewSource;
                }
                else
                {
                    MediaFrameSource recordSource = m_MediaCapture.FrameSources.FirstOrDefault(source => source.Value.Info.MediaStreamType == MediaStreamType.VideoRecord
                                                                                        && source.Value.Info.SourceKind == MediaFrameSourceKind.Color).Value;
                    if (recordSource != null)
                    {
                        m_frameSource = recordSource;
                    }
                }

                if (m_frameSource != null)
                {
                    var formatList = m_frameSource.SupportedFormats;                    
                    foreach (var format in formatList)
                    {
                        // Too slow on my PC for big formats
                        if (format.VideoFormat.Width <= 3000)
                        {
                            MediaFormats.Add(new MediaFrameFormatWrapper(format));
                        }
                    }
                    cmbFrameFormats.IsEnabled = true;
                }
            }
            else
            {
                m_MediaCapture = null;
            }
        }

        private async ValueTask<SoftwareBitmap> ScaleSoftwareBitmapAsync(SoftwareBitmap inputBitmap, uint nNewWidth, uint nNewHeight)
        {        
            using (InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream())
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memoryStream);
                encoder.SetSoftwareBitmap(inputBitmap);
                encoder.BitmapTransform.ScaledWidth = nNewWidth;
                encoder.BitmapTransform.ScaledHeight = nNewHeight;
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Fant; // High quality
                await encoder.FlushAsync();              

                var decoder = await BitmapDecoder.CreateAsync(memoryStream);
                return await decoder.GetSoftwareBitmapAsync(inputBitmap.BitmapPixelFormat, inputBitmap.BitmapAlphaMode);
            }
        }

        IMediaExtension m_MirrorEffect = null, m_MirrorEffectRecord = null;
        IMediaExtension m_GrayScaleD2DEffect = null, m_GrayScaleD2DEffectRecord = null;
        IMediaExtension m_RGBD2DEffect = null, m_RGBD2DEffectRecord = null;
        IMediaExtension m_RotationEffect = null, m_RotationEffectRecord = null;
        IMediaExtension m_InvertD2DEffect = null, m_InvertD2DEffectRecord = null;
        IMediaExtension m_BrightnessEffect = null, m_BrightnessEffectRecord = null;
        IMediaExtension m_OverlayImageEffect = null, m_OverlayImageEffectRecord = null;
        IMediaExtension m_EmbossD2DEffect = null, m_EmbossD2DEffectRecord = null;
        IMediaExtension m_EdgeDetectionD2DEffect = null, m_EdgeDetectionD2DEffectRecord = null;
        IMediaExtension m_GaussianBlurD2DEffect = null, m_GaussianBlurD2DEffectRecord = null;
        IMediaExtension m_SharpenD2DEffect = null, m_SharpenD2DEffectRecord = null;
        IMediaExtension m_VignetteD2DEffect = null, m_VignetteD2DEffectRecord = null;

        FaceDetectionEffect m_FaceDetectionEffect = null;
        
        IMediaExtension m_NullEffect = null;      

        private void SetEffectFloat(ID2D1Effect pEffect, uint nEffect, float fValue)
        {
            float[] aFloatArray = { fValue };
            int nDataSize = aFloatArray.Length * Marshal.SizeOf(typeof(float));
            IntPtr pData = Marshal.AllocHGlobal(nDataSize);
            Marshal.Copy(aFloatArray, 0, pData, aFloatArray.Length);
            HRESULT hr = pEffect.SetValue(nEffect, D2D1_PROPERTY_TYPE.D2D1_PROPERTY_TYPE_UNKNOWN, pData, (uint)nDataSize);
            Marshal.FreeHGlobal(pData);
        }

        private void SetEffectInt(ID2D1Effect pEffect, uint nEffect, uint nValue)
        {
            IntPtr pData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int32)));
            Marshal.WriteInt32(pData, (int)nValue);
            HRESULT hr = pEffect.SetValue(nEffect, D2D1_PROPERTY_TYPE.D2D1_PROPERTY_TYPE_UNKNOWN, pData, (uint)Marshal.SizeOf(typeof(Int32)));
            Marshal.FreeHGlobal(pData);
        }

        private void SetEffectIntPtr(ID2D1Effect pEffect, uint nEffect, IntPtr pPointer)
        {
            IntPtr pData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            Marshal.WriteIntPtr(pData, pPointer);
            HRESULT hr = pEffect.SetValue(nEffect, D2D1_PROPERTY_TYPE.D2D1_PROPERTY_TYPE_UNKNOWN, pData, (uint)Marshal.SizeOf(typeof(IntPtr)));
            Marshal.FreeHGlobal(pData);
        }              

        private async void btn_PreviewVideo_Click(object sender, RoutedEventArgs e)
        {
            if (!m_bPreviewing)
            {
                if (m_frameSource != null)
                {
                    if (m_currentFrameFormat != null)
                    {                        
                        await m_frameSource.SetFormatAsync(m_currentFrameFormat);

                        m_bCameraRender = true;
                        if (!((m_MediaCapture.MediaCaptureSettings.VideoDeviceCharacteristic == VideoDeviceCharacteristic.AllStreamsIdentical ||
                            m_MediaCapture.MediaCaptureSettings.VideoDeviceCharacteristic == VideoDeviceCharacteristic.PreviewRecordStreamsIdentical)
                            && m_bRecording))
                        { 
                            // Test other VideoTransformEffectDefinition
                            //var cropDefinition = new VideoTransformEffectDefinition();
                            //cropDefinition.CropRectangle = new Rect(200, 200, 400, 400);                            
                            //var c  = await m_MediaCapture.AddVideoEffectAsync(cropDefinition, MediaStreamType.VideoPreview);

                            if (cbOverlayImage.IsChecked == true)
                            {
                                var overlayImageEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.OverlayImageEffect");
                                m_OverlayImageEffect = await m_MediaCapture.AddVideoEffectAsync(overlayImageEffectDefinition, MediaStreamType.VideoPreview);
                                //string sExePath = AppContext.BaseDirectory;
                                //string sImagePath = System.IO.Path.Combine(sExePath, "Assets\\Butterfly_Blue_126x100.png");
                                //m_OverlayImageEffect.SetProperties(new PropertySet() { { "ImagePath", sImagePath } });
                                if (m_OverlayImageScaled != null)
                                {
                                    m_OverlayImageScaled.Dispose();
                                    m_OverlayImageScaled = null;
                                }
                                if (m_OverlayImage.PixelWidth > m_currentFrameFormat.VideoFormat.Width / 2)
                                {
                                    double nRatio = (double)m_OverlayImage.PixelWidth / (double)(m_currentFrameFormat.VideoFormat.Width / 2);
                                    m_OverlayImageScaled = await ScaleSoftwareBitmapAsync(m_OverlayImage, m_currentFrameFormat.VideoFormat.Width / 2, (uint)(m_OverlayImage.PixelHeight / nRatio));
                                }
                                else
                                {
                                    m_OverlayImageScaled = SoftwareBitmap.Copy(m_OverlayImage);
                                }
                                m_OverlayImageEffect.SetProperties(new PropertySet() { { "OverlayImage", m_OverlayImageScaled } });
                            }

                            if (tsMirror.IsOn)
                            {
                                var mirrorEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.MirrorEffect");
                                m_MirrorEffect = await m_MediaCapture.AddVideoEffectAsync(mirrorEffectDefinition, MediaStreamType.VideoPreview);
                                m_MirrorEffect.SetProperties(new PropertySet() { { "IsVertical", (m_Mirror == MediaMirroringOptions.Horizontal) ? false : true } });
                            }

                            if (cbEmboss.IsChecked == true)
                            {
                                var embossD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.EmbossD2DEffect");
                                m_EmbossD2DEffect = await m_MediaCapture.AddVideoEffectAsync(embossD2DEffectDefinition, MediaStreamType.VideoPreview);
                                m_EmbossD2DEffect.SetProperties(new PropertySet() { { "StrengthEmboss", m_StrengthEmboss } });
                            }

                            if (cbGaussianBlur.IsChecked == true)
                            {
                                var gaussianBlurD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.GaussianBlurD2DEffect");
                                m_GaussianBlurD2DEffect = await m_MediaCapture.AddVideoEffectAsync(gaussianBlurD2DEffectDefinition, MediaStreamType.VideoPreview);
                                m_GaussianBlurD2DEffect.SetProperties(new PropertySet() { { "DeviationGaussianBlur", m_DeviationGaussianBlur } });                                
                            } 

                            if (cbEdgeDetection.IsChecked == true)
                            {
                                var edgeDetectionD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.EdgeDetectionD2DEffect");
                                m_EdgeDetectionD2DEffect = await m_MediaCapture.AddVideoEffectAsync(edgeDetectionD2DEffectDefinition, MediaStreamType.VideoPreview);
                                m_EdgeDetectionD2DEffect.SetProperties(new PropertySet() { { "StrengthEdgeDetection", m_StrengthEdgeDetection } });
                            }

                            if (tsRotation.IsOn)
                            {
                                // 0xc00d36b4 MF_E_INVALIDMEDIATYPE
                                // "Scaler unavaliable for type"
                                var rotationEffectDefinition = new VideoTransformEffectDefinition();
                                rotationEffectDefinition.Rotation = m_Rotation;
                                m_RotationEffect = await m_MediaCapture.AddVideoEffectAsync(rotationEffectDefinition, MediaStreamType.VideoPreview);
                            }

                            // https://learn.microsoft.com/en-us/windows/apps/develop/camera/scene-analysis-for-media-capture#face-detection-effect
                            if (cbFaceDetection.IsChecked == true)
                            {
                                var faceDetectionEffectDefinition = new FaceDetectionEffectDefinition();
                                faceDetectionEffectDefinition.SynchronousDetectionEnabled = false;
                                faceDetectionEffectDefinition.DetectionMode = FaceDetectionMode.HighQuality;// FaceDetectionMode.HighPerformance;
                                m_FaceDetectionEffect = (FaceDetectionEffect)await m_MediaCapture.AddVideoEffectAsync(faceDetectionEffectDefinition, MediaStreamType.VideoPreview);
                                m_FaceDetectionEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);
                                m_FaceDetectionEffect.Enabled = true;
                                m_FaceDetectionEffect.FaceDetected += FaceDetectionEffect_FaceDetected;
                            }

                            if (cbSharpen.IsChecked == true)
                            {
                                var sharpenD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.SharpenD2DEffect");
                                m_SharpenD2DEffect = await m_MediaCapture.AddVideoEffectAsync(sharpenD2DEffectDefinition, MediaStreamType.VideoPreview);
                                m_SharpenD2DEffect.SetProperties(new PropertySet() { { "SharpnessSharpen", m_SharpnessSharpen } });
                            }

                            if (cbVignette.IsChecked == true)
                            {
                                var vignetteD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.VignetteD2DEffect");
                                m_VignetteD2DEffect = await m_MediaCapture.AddVideoEffectAsync(vignetteD2DEffectDefinition, MediaStreamType.VideoPreview);
                                m_VignetteD2DEffect.SetProperties(new PropertySet()
                                {
                                    { "VignetteStrength", _VignetteStrength },
                                    { "VignetteTransitionSize", _VignetteTransitionSize },
                                    { "VignetteColor", ((SolidColorBrush)rectVignetteColor.Fill).Color }                                    
                                });
                            }

                            if (cbGrayscale.IsChecked == true)
                            {
                                var grayScaleD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.GrayscaleD2DEffect");
                                m_GrayScaleD2DEffect = await m_MediaCapture.AddVideoEffectAsync(grayScaleD2DEffectDefinition, MediaStreamType.VideoPreview);
                            }

                            if (cbRGB.IsChecked == true)
                            {
                                var rgbD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.RGBD2DEffect");
                                m_RGBD2DEffect = await m_MediaCapture.AddVideoEffectAsync(rgbD2DEffectDefinition, MediaStreamType.VideoPreview);
                                m_RGBD2DEffect.SetProperties(new PropertySet() { { "RedValue", m_Red }, { "GreenValue", m_Green }, { "BlueValue", m_Blue } });
                            }

                            if (cbInvert.IsChecked == true)
                            {
                                var invertD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.InvertD2DEffect");
                                m_InvertD2DEffect = await m_MediaCapture.AddVideoEffectAsync(invertD2DEffectDefinition, MediaStreamType.VideoPreview);
                            }

                            if (cbBrightness.IsChecked == true)
                            {
                                var brightnessEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.LuminosityD2DEffect");
                                m_BrightnessEffect = await m_MediaCapture.AddVideoEffectAsync(brightnessEffectDefinition, MediaStreamType.VideoPreview);
                                m_BrightnessEffect.SetProperties(new PropertySet()
                                {
                                    { "BrightnessWhitePointX", _BrightnessWhitePointX },
                                    { "BrightnessWhitePointY", _BrightnessWhitePointY },
                                    { "BrightnessBlackPointX", _BrightnessBlackPointX },
                                    { "BrightnessBlackPointY", _BrightnessBlackPointY }
                                });
                            }

                            // If no custom effect, Brightness is too high with VideoProcessor : Color space or gamma mismatch ?
                            if (!(cbOverlayImage.IsChecked == true || tsMirror.IsOn || cbGrayscale.IsChecked == true ||
                             cbRGB.IsChecked == true || cbInvert.IsChecked == true || cbBrightness.IsChecked == true ||
                             cbEmboss.IsChecked == true || cbEdgeDetection.IsChecked == true || cbGaussianBlur.IsChecked == true || 
                             cbSharpen.IsChecked == true || cbVignette.IsChecked == true) || (tsRotation.IsOn || cbFaceDetection.IsChecked == true))
                            {
                                var nullEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.NullEffect");
                                m_NullEffect = await m_MediaCapture.AddVideoEffectAsync(nullEffectDefinition, MediaStreamType.VideoPreview);
                            }
                        }

                        //await m_frameSource.SetFormatAsync(m_currentFrameFormat);
                        m_FrameReader = await m_MediaCapture.CreateFrameReaderAsync(m_frameSource/*, MediaEncodingSubtypes.Bgra8*/);                        
                        m_FrameReader.FrameArrived += FrameReader_FrameArrived;
                        await m_FrameReader.StartAsync();
                        m_bPreviewing = true;
                        cmbDevices.IsEnabled = false;
                        cmbFrameFormats.IsEnabled = false;
                        var disabledBrush = (Brush)Application.Current.Resources["ButtonForegroundDisabled"];
                        tbDevices.Foreground = disabledBrush;
                        tbFrameFormats.Foreground = disabledBrush;
                        EnableChildControls(sp_Effects, false);
                        btn_PreviewVideo.Background = new SolidColorBrush(Microsoft.UI.Colors.LightGreen);
                    }
                    else
                    {
                        Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("No frame format selected", "Information");
                        WinRT.Interop.InitializeWithWindow.Initialize(md, hWndMain);
                        _ = await md.ShowAsync();
                    }
                }
                else
                {
                    Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("No device selected", "Information");
                    WinRT.Interop.InitializeWithWindow.Initialize(md, hWndMain);
                    _ = await md.ShowAsync();
                }
            }
            else
            {
                if (m_frameSource != null)
                {                    
                    StopFrameReader(true, false);
                    m_bCameraRender = false;
                }
            }
        }
       
        private List<BitmapBounds> m_DetectedFaces = new List<BitmapBounds>();

        private void FaceDetectionEffect_FaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            m_DetectedFaces.Clear();
            if (args.ResultFrame.DetectedFaces.Count > 0)
            {
                //m_DetectedFaces.Clear();
                foreach (Windows.Media.FaceAnalysis.DetectedFace face in args.ResultFrame.DetectedFaces)
                {
                    BitmapBounds faceRect = face.FaceBox;
                    m_DetectedFaces.Add(faceRect);
                    //Console.Beep(3000, 10);
                }               
            }
        }         

        D3D11.ID3D11Texture2D m_pSharedTexture = null;
        IntPtr m_SharedHandle = IntPtr.Zero;

        bool m_bInitTexture = false;
        ID3D11VideoProcessorEnumerator m_pVideoProcessorEnumerator = null;
        bool m_bInitVideoProcessorEnumerator = false;
        ID3D11VideoProcessor m_pVideoProcessor = null;
        bool m_bInitVideoProcessor = false;    

        private void FrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        { 
            if (!m_bPreviewing)
                return;
            try
            {
                HRESULT hr = HRESULT.S_OK;
                using (var frame = sender.TryAcquireLatestFrame())
                {
                    if (frame?.VideoMediaFrame != null)
                    {
                        try
                        {
                            var pDirect3DSurface = frame?.VideoMediaFrame.Direct3DSurface;
                            if (pDirect3DSurface != null)
                            {                                
                                IntPtr pTexturePtr = IntPtr.Zero;
                                var dia = pDirect3DSurface.As<IDirect3DDxgiInterfaceAccess>();
                                Guid guid = typeof(D3D11.ID3D11Texture2D).GUID;
                                hr = dia.GetInterface(guid, out pTexturePtr);
                                D3D11.ID3D11Texture2D pOriginalTexture = null;
                                if (hr == HRESULT.S_OK && pTexturePtr != IntPtr.Zero)
                                {
                                    pOriginalTexture = (D3D11.ID3D11Texture2D)Marshal.GetObjectForIUnknown(pTexturePtr);
                                    Marshal.Release(pTexturePtr);                                                                   
                                    if (pOriginalTexture != null)
                                    {
                                        ID3D11Device pD3D11Device = null;
                                        pOriginalTexture.GetDevice(out pD3D11Device);
                                        if (pD3D11Device != null)
                                        {
                                            // Format = DXGI_FORMAT_YUY2
                                            // MiscFlags = 2050 = 0x802 = D3D11_RESOURCE_MISC_SHARED | D3D11_RESOURCE_MISC_SHARED_NTHANDLE 
                                            var textureDesc = new D3D11.D3D11_TEXTURE2D_DESC();
                                            pOriginalTexture.GetDesc(out textureDesc);

                                            textureDesc.Usage = /*D3D11.D3D11_USAGE.D3D11_USAGE_STAGING;*/ D3D11.D3D11_USAGE.D3D11_USAGE_DEFAULT;
                                            textureDesc.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET;
                                            textureDesc.CPUAccessFlags = 0;// D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_READ;
                                            textureDesc.MiscFlags = D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_SHARED /*| D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_SHARED_NTHANDLE*/  /*| D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_SHARED_KEYEDMUTEX*/;
                                            textureDesc.Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;

                                            if (!m_bInitTexture)
                                            {
                                                hr = pD3D11Device.CreateTexture2D(ref textureDesc, IntPtr.Zero, out m_pSharedTexture);
                                                if (hr == HRESULT.S_OK)
                                                    m_bInitTexture = true;
                                            }

                                            if (hr == HRESULT.S_OK)
                                            {
                                                D3D11.ID3D11DeviceContext pD3D11DeviceContext = null;
                                                pD3D11Device.GetImmediateContext(out pD3D11DeviceContext);
                                                if (pD3D11DeviceContext != null)
                                                {
                                                    pD3D11DeviceContext.CopyResource(m_pSharedTexture, pOriginalTexture);
                                                    pD3D11DeviceContext.Flush(); // Forces GPU to complete pending commands

                                                    //var sharedTextureDesc = new D3D11.D3D11_TEXTURE2D_DESC();
                                                    //m_pSharedTexture.GetDesc(out sharedTextureDesc);
                                                    if (m_pSharedTexture != null)
                                                    {
                                                        IDXGIResource pDXGIResource = m_pSharedTexture as IDXGIResource;
                                                        hr = pDXGIResource.GetSharedHandle(out m_SharedHandle);
                                                        if (hr == HRESULT.S_OK)
                                                        {
                                                           
                                                        }
                                                    }
                                                    SafeRelease(ref pD3D11DeviceContext);
                                                }
                                            }
                                            SafeRelease(ref pD3D11Device);
                                        }
                                        SafeRelease(ref pOriginalTexture);                                        
                                    }                                   
                                }
                            }
                        }
                        catch (OutOfMemoryException e)
                        {
                            System.Diagnostics.Debug.WriteLine("Terminating application unexpectedly... (reading Direct3DSurface)");
                            Environment.FailFast(String.Format("Out of Memory: {0}", e.Message));
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"\"Error reading Direct3DSurface : {ex.Message}");
                            StopFrameReader(true, true);                            
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"\"Error TryAcquireLatestFrame : {ex.Message}");
                StopFrameReader(true, true);                
            }
        }

        // Old test with VideoProcessor : too bright if no custom effect
        private void FrameReader_FrameArrived2(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            if (!m_bPreviewing) return;

            try
            {
                using (var frame = sender.TryAcquireLatestFrame())
                {
                    if (frame?.VideoMediaFrame != null)
                    {
                        try
                        {
                            var direct3DSurface = frame?.VideoMediaFrame.Direct3DSurface;
                            IDirect3DDxgiInterfaceAccess dia = direct3DSurface.As<IDirect3DDxgiInterfaceAccess>();
                            IntPtr pDXGISurfacePtr = IntPtr.Zero;
                            Guid idxgiSurfaceGuid = typeof(IDXGISurface).GUID;
                            HRESULT hr = dia.GetInterface(idxgiSurfaceGuid, out pDXGISurfacePtr);
                            if (hr == HRESULT.S_OK)
                            {
                                IDXGISurface pDXGISurface = Marshal.GetObjectForIUnknown(pDXGISurfacePtr) as IDXGISurface;

                                DXGI_SURFACE_DESC desc;
                                pDXGISurface.GetDesc(out desc);
                                //Debug.WriteLine($"DXGI Surface Format: {desc.Format}, Width: {desc.Width}, Height: {desc.Height}");
                                //DXGI Surface Format: DXGI_FORMAT_YUY2, Width: 640, Height: 480

                                IntPtr pDevicePtr = IntPtr.Zero;
                                Guid IID_ID3D11Device = typeof(ID3D11Device).GUID;
                                hr = pDXGISurface.GetDevice(IID_ID3D11Device, out pDevicePtr);
                                if (hr == HRESULT.S_OK)
                                {
                                    ID3D11Device pD3D11Device = Marshal.GetObjectForIUnknown(pDevicePtr) as ID3D11Device;
                                    if (pD3D11Device != null)
                                    {
                                        D3D11.ID3D11Texture2D pOriginalTexture = (D3D11.ID3D11Texture2D)Marshal.GetTypedObjectForIUnknown(pDXGISurfacePtr, typeof(D3D11.ID3D11Texture2D));

                                        var textureDesc = new D3D11.D3D11_TEXTURE2D_DESC();
                                        pOriginalTexture.GetDesc(out textureDesc);
                                        //DXGI Surface Format: DXGI_FORMAT_YUY2, Width: 640, Height: 480
                                        //DXGI Surface Format: DXGI_FORMAT_NV12, Width: 640, Height: 480
                                        //Debug.WriteLine($"DXGI Surface Format: {textureDesc.Format}, Width: {textureDesc.Width}, Height: {textureDesc.Height}");

                                        textureDesc.Usage = /*D3D11.D3D11_USAGE.D3D11_USAGE_STAGING;*/ D3D11.D3D11_USAGE.D3D11_USAGE_DEFAULT;
                                        textureDesc.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET;
                                        textureDesc.CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_READ;
                                        textureDesc.MiscFlags = D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_SHARED /*| D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_SHARED_KEYEDMUTEX*/;
                                        textureDesc.Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;

                                        if (!m_bInitTexture)
                                            hr = pD3D11Device.CreateTexture2D(ref textureDesc, IntPtr.Zero, out m_pSharedTexture);
                                        else
                                            m_bInitTexture = true;
                                        if (hr == HRESULT.S_OK)
                                        {
                                            D3D11.ID3D11DeviceContext pD3D11DeviceContext = null;
                                            pD3D11Device.GetImmediateContext(out pD3D11DeviceContext);

                                            pD3D11DeviceContext.CopyResource(m_pSharedTexture, pOriginalTexture);

                                            //IDXGIKeyedMutex keyedMutex;
                                            //keyedMutex = (IDXGIKeyedMutex)m_pSharedTexture;
                                            //hr = keyedMutex.AcquireSync(0, 0xFFFFFFFF/*INFINITE*/);
                                            //pD3D11DeviceContext.CopyResource(m_pSharedTexture, pOriginalTexture);
                                            //hr = keyedMutex.ReleaseSync(0);

                                            ID3D11VideoDevice pD3D11VideoDevice = Marshal.GetObjectForIUnknown(pDevicePtr) as ID3D11VideoDevice;
                                            var contentDesc = new D3D11_VIDEO_PROCESSOR_CONTENT_DESC
                                            {
                                                InputFrameFormat = D3D11_VIDEO_FRAME_FORMAT.D3D11_VIDEO_FRAME_FORMAT_INTERLACED_TOP_FIELD_FIRST, // Input format (interlaced or progressive)
                                                InputWidth = textureDesc.Width,
                                                InputHeight = textureDesc.Height,
                                                OutputWidth = textureDesc.Width,
                                                OutputHeight = textureDesc.Height,
                                                Usage = D3D11_VIDEO_USAGE.D3D11_VIDEO_USAGE_OPTIMAL_SPEED
                                            };
                                            if (!m_bInitVideoProcessorEnumerator)
                                                hr = pD3D11VideoDevice.CreateVideoProcessorEnumerator(ref contentDesc, out m_pVideoProcessorEnumerator);
                                            else
                                                m_bInitVideoProcessorEnumerator = true;
                                            if (hr == HRESULT.S_OK)
                                            {
                                                //                                     //uint nFlags;
                                                //                                     //hr = pVideoProcessorEnum.CheckVideoProcessorFormat(desc.Format, out nFlags);
                                                //                                     //if ( 0 == (nFlags & (uint)D3D11_VIDEO_PROCESSOR_FORMAT_SUPPORT.D3D11_VIDEO_PROCESSOR_FORMAT_SUPPORT_INPUT))
                                                //                                     //{
                                                //                                     //    return;// HRESULT.E_INVALIDARG;
                                                //                                     //}

                                                //                                     D3D11_VIDEO_PROCESSOR_CAPS vpCaps = new D3D11_VIDEO_PROCESSOR_CAPS();
                                                //                                     hr = pVideoProcessorEnum.GetVideoProcessorCaps(out vpCaps);

                                                //                                     /*
                                                //                                      VideoProcessorCaps:
                                                //Device YCbCr matrix conversion: supported
                                                //Device xvYCC                  : supported
                                                //Device YUV nominal range      : supported
                                                //Device RGB Range Conversion   : supported
                                                //Feature LEGACY                : No
                                                //Feature Shader usage          : NOT supported
                                                //Feature Metadata HDR10        : NOT supported
                                                //Filter capabilities           : Brightness, Contrast, Hue, Saturation, Noise reduction, Edge enhancement, Anamorphic scaling
                                                //InputFormat interlaced RGB    : NOT supported
                                                //InputFormat RGB ProcAmp       : NOT supported
                                                //InputFormat RGB Luma Key      : NOT supported
                                                //AutoStream image processing   : Denoise
                                                //                                      

                                                //                                     //#if DEBUG
                                                //                                     //                                            {
                                                //                                     //                                                var dbgstr = "VideoProcessorCaps:";
                                                //                                     //                                                dbgstr += $"\n  Device YCbCr matrix conversion: {((vpCaps.DeviceCaps & (uint)D3D11_VIDEO_PROCESSOR_DEVICE_CAPS.D3D11_VIDEO_PROCESSOR_DEVICE_CAPS_YCbCr_MATRIX_CONVERSION) != 0 ? "supported" : "NOT supported")}";
                                                //                                     //                                                dbgstr += $"\n  Device xvYCC                  : {((vpCaps.DeviceCaps & (uint)D3D11_VIDEO_PROCESSOR_DEVICE_CAPS.D3D11_VIDEO_PROCESSOR_DEVICE_CAPS_xvYCC) != 0 ? "supported" : "NOT supported")}";
                                                //                                     //                                                dbgstr += $"\n  Device YUV nominal range      : {((vpCaps.DeviceCaps & (uint)D3D11_VIDEO_PROCESSOR_DEVICE_CAPS.D3D11_VIDEO_PROCESSOR_DEVICE_CAPS_NOMINAL_RANGE) != 0 ? "supported" : "NOT supported")}";
                                                //                                     //                                                dbgstr += $"\n  Device RGB Range Conversion   : {((vpCaps.DeviceCaps & (uint)D3D11_VIDEO_PROCESSOR_DEVICE_CAPS.D3D11_VIDEO_PROCESSOR_DEVICE_CAPS_RGB_RANGE_CONVERSION) != 0 ? "supported" : "NOT supported")}";
                                                //                                     //                                                dbgstr += $"\n  Feature LEGACY                : {((vpCaps.FeatureCaps & (uint)D3D11_VIDEO_PROCESSOR_FEATURE_CAPS.D3D11_VIDEO_PROCESSOR_FEATURE_CAPS_LEGACY) != 0 ? "Yes" : "No")}";
                                                //                                     //                                                dbgstr += $"\n  Feature Shader usage          : {((vpCaps.FeatureCaps & (uint)D3D11_VIDEO_PROCESSOR_FEATURE_CAPS.D3D11_VIDEO_PROCESSOR_FEATURE_CAPS_SHADER_USAGE) != 0 ? "supported" : "NOT supported")}";
                                                //                                     //                                                dbgstr += $"\n  Feature Metadata HDR10        : {((vpCaps.FeatureCaps & (uint)D3D11_VIDEO_PROCESSOR_FEATURE_CAPS.D3D11_VIDEO_PROCESSOR_FEATURE_CAPS_METADATA_HDR10) != 0 ? "supported" : "NOT supported")}";

                                                //                                     //                                                dbgstr += "\n  Filter capabilities           :";
                                                //                                     //                                                if ((vpCaps.FilterCaps & (uint)D3D11_VIDEO_PROCESSOR_FILTER_CAPS.D3D11_VIDEO_PROCESSOR_FILTER_CAPS_BRIGHTNESS) != 0) dbgstr += " Brightness,";
                                                //                                     //                                                if ((vpCaps.FilterCaps & (uint)D3D11_VIDEO_PROCESSOR_FILTER_CAPS.D3D11_VIDEO_PROCESSOR_FILTER_CAPS_CONTRAST) != 0) dbgstr += " Contrast,";
                                                //                                     //                                                if ((vpCaps.FilterCaps & (uint)D3D11_VIDEO_PROCESSOR_FILTER_CAPS.D3D11_VIDEO_PROCESSOR_FILTER_CAPS_HUE) != 0) dbgstr += " Hue,";
                                                //                                     //                                                if ((vpCaps.FilterCaps & (uint)D3D11_VIDEO_PROCESSOR_FILTER_CAPS.D3D11_VIDEO_PROCESSOR_FILTER_CAPS_SATURATION) != 0) dbgstr += " Saturation,";
                                                //                                     //                                                if ((vpCaps.FilterCaps & (uint)D3D11_VIDEO_PROCESSOR_FILTER_CAPS.D3D11_VIDEO_PROCESSOR_FILTER_CAPS_NOISE_REDUCTION) != 0) dbgstr += " Noise reduction,";
                                                //                                     //                                                if ((vpCaps.FilterCaps & (uint)D3D11_VIDEO_PROCESSOR_FILTER_CAPS.D3D11_VIDEO_PROCESSOR_FILTER_CAPS_EDGE_ENHANCEMENT) != 0) dbgstr += " Edge enhancement,";
                                                //                                     //                                                if ((vpCaps.FilterCaps & (uint)D3D11_VIDEO_PROCESSOR_FILTER_CAPS.D3D11_VIDEO_PROCESSOR_FILTER_CAPS_ANAMORPHIC_SCALING) != 0) dbgstr += " Anamorphic scaling,";
                                                //                                     //                                                if ((vpCaps.FilterCaps & (uint)D3D11_VIDEO_PROCESSOR_FILTER_CAPS.D3D11_VIDEO_PROCESSOR_FILTER_CAPS_STEREO_ADJUSTMENT) != 0) dbgstr += " Stereo adjustment";

                                                //                                     //                                                dbgstr = dbgstr.TrimEnd(',');

                                                //                                     //                                                dbgstr += $"\n  InputFormat interlaced RGB    : {((vpCaps.InputFormatCaps & (uint)D3D11_VIDEO_PROCESSOR_FORMAT_CAPS.D3D11_VIDEO_PROCESSOR_FORMAT_CAPS_RGB_INTERLACED) != 0 ? "supported" : "NOT supported")}";
                                                //                                     //                                                dbgstr += $"\n  InputFormat RGB ProcAmp       : {((vpCaps.InputFormatCaps & (uint)D3D11_VIDEO_PROCESSOR_FORMAT_CAPS.D3D11_VIDEO_PROCESSOR_FORMAT_CAPS_RGB_PROCAMP) != 0 ? "supported" : "NOT supported")}";
                                                //                                     //                                                dbgstr += $"\n  InputFormat RGB Luma Key      : {((vpCaps.InputFormatCaps & (uint)D3D11_VIDEO_PROCESSOR_FORMAT_CAPS.D3D11_VIDEO_PROCESSOR_FORMAT_CAPS_RGB_LUMA_KEY) != 0 ? "supported" : "NOT supported")}";

                                                //                                     //                                                dbgstr += "\n  AutoStream image processing   :";
                                                //                                     //                                                if (vpCaps.AutoStreamCaps == 0)
                                                //                                     //                                                {
                                                //                                     //                                                    dbgstr += " None";
                                                //                                     //                                                }
                                                //                                     //                                                else
                                                //                                     //                                                {
                                                //                                     //                                                    if ((vpCaps.AutoStreamCaps & (uint)D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS.D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS_DENOISE) != 0) dbgstr += " Denoise,";
                                                //                                     //                                                    if ((vpCaps.AutoStreamCaps & (uint)D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS.D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS_DERINGING) != 0) dbgstr += " Deringing,";
                                                //                                     //                                                    if ((vpCaps.AutoStreamCaps & (uint)D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS.D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS_EDGE_ENHANCEMENT) != 0) dbgstr += " Edge enhancement,";
                                                //                                     //                                                    if ((vpCaps.AutoStreamCaps & (uint)D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS.D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS_COLOR_CORRECTION) != 0) dbgstr += " Color correction,";
                                                //                                     //                                                    if ((vpCaps.AutoStreamCaps & (uint)D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS.D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS_FLESH_TONE_MAPPING) != 0) dbgstr += " Flesh tone mapping,";
                                                //                                     //                                                    if ((vpCaps.AutoStreamCaps & (uint)D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS.D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS_IMAGE_STABILIZATION) != 0) dbgstr += " Image stabilization,";
                                                //                                     //                                                    if ((vpCaps.AutoStreamCaps & (uint)D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS.D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS_SUPER_RESOLUTION) != 0) dbgstr += " Super resolution,";
                                                //                                     //                                                    if ((vpCaps.AutoStreamCaps & (uint)D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS.D3D11_VIDEO_PROCESSOR_AUTO_STREAM_CAPS_ANAMORPHIC_SCALING) != 0) dbgstr += " Anamorphic scaling";

                                                //                                     //                                                    dbgstr = dbgstr.TrimEnd(',');
                                                //                                     //                                                }

                                                //                                     //                                                Debug.WriteLine(dbgstr);
                                                //                                     //                                            }
                                                //                                     //#endif

                                                if (!m_bInitVideoProcessor)
                                                    hr = pD3D11VideoDevice.CreateVideoProcessor(m_pVideoProcessorEnumerator, 0, out m_pVideoProcessor);
                                                else
                                                    m_bInitVideoProcessor = true;
                                                if (hr == HRESULT.S_OK)
                                                {
                                                    IntPtr pVideoContextPtr = IntPtr.Zero;
                                                    Guid videoContextGuid = new Guid("61F21C45-3C0E-4a74-9CEA-67100D9AD5E4");
                                                    int nRet = Marshal.QueryInterface(Marshal.GetIUnknownForObject(pD3D11DeviceContext), ref videoContextGuid, out pVideoContextPtr);
                                                    var pVideoContext = (ID3D11VideoContext)Marshal.GetObjectForIUnknown(pVideoContextPtr);

                                                    var inputViewDesc = new D3D11_VIDEO_PROCESSOR_INPUT_VIEW_DESC
                                                    {
                                                        FourCC = 0,
                                                        ViewDimension = D3D11_VPIV_DIMENSION.D3D11_VPIV_DIMENSION_TEXTURE2D,
                                                        Texture2D = new D3D11_TEX2D_VPIV { MipSlice = 0 }
                                                    };
                                                    ID3D11VideoProcessorInputView pInputView = null;
                                                    hr = pD3D11VideoDevice.CreateVideoProcessorInputView(pOriginalTexture, m_pVideoProcessorEnumerator, ref inputViewDesc, out pInputView);
                                                    if (hr == HRESULT.S_OK)
                                                    {
                                                        // Does nothing ?
                                                        //D3D11_VIDEO_PROCESSOR_FILTER_RANGE pRange = new D3D11_VIDEO_PROCESSOR_FILTER_RANGE();
                                                        //hr = pVideoProcessorEnum.GetVideoProcessorFilterRange(D3D11_VIDEO_PROCESSOR_FILTER.D3D11_VIDEO_PROCESSOR_FILTER_BRIGHTNESS, out pRange);
                                                        ////int nVal = pRange.Default + (pRange.Minimum - pRange.Default) * (75 - 50) / (0 - 50);
                                                        //pVideoContext.VideoProcessorSetStreamFilter(pVideoProcessor, 0, D3D11_VIDEO_PROCESSOR_FILTER.D3D11_VIDEO_PROCESSOR_FILTER_BRIGHTNESS, true, -100);

                                                        // OK
                                                        //pVideoContext.VideoProcessorSetStreamRotation(pVideoProcessor, 0, true, D3D11_VIDEO_PROCESSOR_ROTATION.D3D11_VIDEO_PROCESSOR_ROTATION_90);
                                                        //pVideoContext.VideoProcessorSetStreamAlpha(pVideoProcessor, 0, true, 0.1f);

                                                        // Does Nothing...
                                                        //D3D11_VIDEO_PROCESSOR_COLOR_SPACE vpcs = new D3D11_VIDEO_PROCESSOR_COLOR_SPACE()
                                                        //{
                                                        //    Usage = false,
                                                        //    RGB_Range = true, // Ensure full RGB range (0-255)
                                                        //    //Nominal_Range = (byte)D3D11_VIDEO_PROCESSOR_NOMINAL_RANGE.D3D11_VIDEO_PROCESSOR_NOMINAL_RANGE_0_255,
                                                        //    Nominal_Range = (byte)D3D11_VIDEO_PROCESSOR_NOMINAL_RANGE.D3D11_VIDEO_PROCESSOR_NOMINAL_RANGE_16_235,
                                                        //    YCbCr_Matrix = false, // Use BT.709 color matrix (for HD content)
                                                        //    YCbCr_xvYCC = false,
                                                        //    //Reserved = 0
                                                        //};

                                                        //// Set correct color space for output and input                                                        
                                                        //pVideoContext.VideoProcessorSetStreamColorSpace(pVideoProcessor, 0, ref vpcs);
                                                        ////vpcs.YCbCr_Matrix = 1;
                                                        //vpcs.YCbCr_xvYCC = true;
                                                        ////vpcs.Nominal_Range = (uint)D3D11_VIDEO_PROCESSOR_NOMINAL_RANGE.D3D11_VIDEO_PROCESSOR_NOMINAL_RANGE_16_235;
                                                        //pVideoContext.VideoProcessorSetOutputColorSpace(pVideoProcessor, ref vpcs);

                                                        // Does Nothing...
                                                        //D3D11_VIDEO_COLOR vc = new D3D11_VIDEO_COLOR();
                                                        ////vc.__field__0.RGBA.G = 1.0f;
                                                        //vc.RGBA.R = 1.0f;
                                                        //vc.RGBA.A = 0.5f;
                                                        //vc.YCbCr.Y = 1.0f;
                                                        //vc.YCbCr.Cb = 1.0f;
                                                        //vc.YCbCr.Cr = 1.0f;
                                                        //vc.YCbCr.A = 0.5f;
                                                        //pVideoContext.VideoProcessorSetOutputBackgroundColor(pVideoProcessor, true, ref vc);
                                                        //pVideoContext.VideoProcessorSetOutputAlphaFillMode(pVideoProcessor, D3D11_VIDEO_PROCESSOR_ALPHA_FILL_MODE.D3D11_VIDEO_PROCESSOR_ALPHA_FILL_MODE_BACKGROUND, 0);

                                                        // OK
                                                        //RECT rect = new RECT(0, 0, 200, 150);
                                                        //pVideoContext.VideoProcessorSetStreamDestRect(pVideoProcessor, 0, true, ref rect);

                                                        var outputViewDesc = new D3D11_VIDEO_PROCESSOR_OUTPUT_VIEW_DESC
                                                        {
                                                            ViewDimension = D3D11_VPOV_DIMENSION.D3D11_VPOV_DIMENSION_TEXTURE2D,
                                                            Texture2D = new D3D11_TEX2D_VPOV { MipSlice = 0 }
                                                        };

                                                        ID3D11VideoProcessorOutputView pOutputView = null;
                                                        hr = pD3D11VideoDevice.CreateVideoProcessorOutputView(m_pSharedTexture, m_pVideoProcessorEnumerator, ref outputViewDesc, out pOutputView);
                                                        if (hr == HRESULT.S_OK)
                                                        {
                                                            // Does Nothing...
                                                            // float gamma = 22.2f;
                                                            // pVideoContext.VideoProcessorSetStreamLumaKey(pVideoProcessor, 0, true, -20.0f, gamma);

                                                            // SIZE sz = new SIZE(100, 100);
                                                            // pVideoContext.VideoProcessorSetOutputConstriction(pVideoProcessor, true, sz);

                                                            D3D11_VIDEO_PROCESSOR_STREAM stream = new D3D11_VIDEO_PROCESSOR_STREAM()
                                                            {
                                                                Enable = true,
                                                                OutputIndex = 0,
                                                                InputFrameOrField = 0,
                                                                PastFrames = 0,
                                                                FutureFrames = 0,
                                                                ppPastSurfaces = IntPtr.Zero,
                                                                pInputSurface = pInputView,
                                                                ppFutureSurfaces = IntPtr.Zero
                                                            };
                                                            // If Effect and resizing
                                                            // D3D11 ERROR: ID3D11Device::OpenSharedResource: Returning E_INVALIDARG, meaning invalid parameters were passed. [ STATE_CREATION ERROR #381: DEVICE_OPEN_SHARED_RESOURCE_INVALIDARG_RETURN]
                                                            hr = pVideoContext.VideoProcessorBlt(m_pVideoProcessor, pOutputView, 0, 1, new[] { stream });

                                                            SafeRelease(ref pOutputView);
                                                        }
                                                        SafeRelease(ref pInputView);
                                                    }
                                                    SafeRelease(ref pVideoContext);
                                                    if (pVideoContextPtr != IntPtr.Zero)
                                                        Marshal.Release(pVideoContextPtr);
                                                }                                               
                                            }
                                            SafeRelease(ref pD3D11VideoDevice);
                                            if (hr == HRESULT.S_OK && m_pSharedTexture != null)
                                            {
                                                IDXGIResource dxgiResource = m_pSharedTexture as IDXGIResource;
                                                hr = dxgiResource.GetSharedHandle(out m_SharedHandle);
                                                //pD3D11DeviceContext.Flush();
                                                if (hr != HRESULT.S_OK)
                                                {
                                                    //int i = 1;
                                                }
                                            }
                                            SafeRelease(ref pD3D11DeviceContext);
                                        }
                                        SafeRelease(ref pD3D11Device);
                                        SafeRelease(ref pOriginalTexture);
                                    }
                                    if (pDevicePtr != IntPtr.Zero)
                                        Marshal.Release(pDevicePtr);
                                }
                                SafeRelease(ref pDXGISurface);
                                if (pDXGISurfacePtr != IntPtr.Zero)
                                    Marshal.Release(pDXGISurfacePtr);                                                                                           
                            }
                        }
                        catch (OutOfMemoryException e)
                        {
                            System.Diagnostics.Debug.WriteLine("Terminating application unexpectedly... (reading Direct3DSurface)");
                            Environment.FailFast(String.Format("Out of Memory: {0}", e.Message));
                        }
                        catch (System.Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"\"Error reading Direct3DSurface : {ex.Message}");
                            StopFrameReader(true, true);                            
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"\"Error TryAcquireLatestFrame : {ex.Message}");
                StopFrameReader(true, true);                
            }
        }

        //public IAsyncAction CloseAsync()
        //{
        //    return Task.Run(async () =>
        //    {
        //        m_bPreviewing = false;
        //        m_FrameReader.FrameArrived -= FrameReader_FrameArrived;                
        //        await m_FrameReader.StopAsync();
        //        //m_FrameReader.Dispose();
        //    }).AsAsyncAction();
        //}

        private async Task CloseAsync()
        {
            m_bPreviewing = false;
            try
            {
                if (m_FrameReader != null)
                {
                    await m_FrameReader.StopAsync();
                    m_FrameReader.FrameArrived -= FrameReader_FrameArrived;                    
                    m_FrameReader.Dispose();
                    m_FrameReader = null;
                }
            }
            catch (Exception ex)
            {               
                Debug.WriteLine($"Error during CloseAsync: {ex.Message}");
            }
        }

        /*
         * if RemoveEffectAsync :
            WinRT originate error - 0x80004003 : 'The source object being cast to type 'VideoEffectComponent.GrayscaleD2DEffect+IDirect3DDxgiInterfaceAccess' is 'null'. (Parameter 'value')'.
            WinRT originate error - 0xC00DABE5 : 'Les effets précédemment ajoutés étaient incompatibles avec la nouvelle topologie. Tous les effets ont été supprimés.'.
          */

        // MF_E_INVALIDREQUEST
        // WinRT originate error - 0xC00D36B2 : 'La demande est incorrecte dans l'état actuel.'.

        //private async Task StopFrameReader(bool bEnableControls, bool bInFrame)
        private async void StopFrameReader(bool bEnableControls, bool bInFrame)
        {
            if (m_FrameReader != null && m_MediaCapture != null)
            {
                //System.Diagnostics.Debug.WriteLine($"\"CameraStreamState : {m_MediaCapture.CameraStreamState}");               
                await CloseAsync();

                //m_FrameReader.FrameArrived -= FrameReader_FrameArrived;
                //m_bPreviewing = false;
                //await m_FrameReader.StopAsync();                

                //System.Threading.Thread.Sleep(100);
                await m_MediaCapture.ClearEffectsAsync(MediaStreamType.VideoPreview);

                if (m_MirrorEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_MirrorEffect);
                    m_MirrorEffect = null;
                }

                if (m_EmbossD2DEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_EmbossD2DEffect);
                    m_EmbossD2DEffect = null;
                }

                if (m_EdgeDetectionD2DEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_EdgeDetectionD2DEffect);
                    m_EdgeDetectionD2DEffect = null;                  
                }

                if (m_GaussianBlurD2DEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_GaussianBlurD2DEffect);
                    m_GaussianBlurD2DEffect = null;
                }

                if (m_SharpenD2DEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_SharpenD2DEffect);
                    m_SharpenD2DEffect = null;
                }

                if (m_VignetteD2DEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_VignetteD2DEffect);
                    m_VignetteD2DEffect = null;
                }

                if (m_FaceDetectionEffect != null)
                {
                    m_FaceDetectionEffect.Enabled = false;
                    m_FaceDetectionEffect.FaceDetected -= FaceDetectionEffect_FaceDetected;
                    //await m_MediaCapture.RemoveEffectAsync(m_FaceDetectionEffect);
                    m_FaceDetectionEffect = null;
                    m_DetectedFaces.Clear();
                }

                if (m_RotationEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_RotationEffect);
                    m_RotationEffect = null;
                }

                if (m_GrayScaleD2DEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_GrayScaleD2DEffect);
                    m_GrayScaleD2DEffect = null;
                }

                if (m_RGBD2DEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_RGBD2DEffect);
                    m_RGBD2DEffect = null;
                }

                if (m_InvertD2DEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_InvertD2DEffect);
                    m_InvertD2DEffect = null;
                }

                if (m_BrightnessEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_BrightnessEffect);
                    m_BrightnessEffect = null;
                }

                if (m_OverlayImageEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_OverlayImageEffect);
                    m_OverlayImageEffect = null;
                }

                if (m_NullEffect != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_NullEffect);
                    m_NullEffect = null;
                }                
            }            
            m_bInitTexture = false;
            m_bInitVideoProcessor = false;
            SafeRelease(ref m_pVideoProcessor);
            m_bInitVideoProcessorEnumerator = false;
            SafeRelease(ref m_pVideoProcessorEnumerator);
            //m_MediaCapture.Dispose();
            //m_MediaCapture = null;
            if (bEnableControls)
            {
                if (!bInFrame)
                {
                    if (!m_bRecording)
                    {
                        cmbDevices.IsEnabled = true;
                        cmbFrameFormats.IsEnabled = true;
                        var defaultBrush = (Brush)Application.Current.Resources["DefaultTextForegroundThemeBrush"];                        
                        tbDevices.Foreground = defaultBrush;
                        tbFrameFormats.Foreground = defaultBrush;
                        EnableChildControls(sp_Effects, true);
                    }
                    btn_PreviewVideo.Background = new SolidColorBrush(Microsoft.UI.Colors.MidnightBlue);
                }
                else
                {
                    bool isQueued = this.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        if (!m_bRecording)
                        {
                            cmbDevices.IsEnabled = true;
                            cmbFrameFormats.IsEnabled = true;
                            var defaultBrush = (Brush)Application.Current.Resources["DefaultTextForegroundThemeBrush"];
                            tbDevices.Foreground = defaultBrush;
                            tbFrameFormats.Foreground = defaultBrush;
                            EnableChildControls(sp_Effects, true);
                        }
                        btn_PreviewVideo.Background = new SolidColorBrush(Microsoft.UI.Colors.MidnightBlue);
                    });
                }            
            }
        }

        private void EnableChildControls(StackPanel panel, bool bEnabled)
        {
            foreach (var child in panel.Children)
            {
                if (child is Control control)
                {
                    control.IsEnabled = bEnabled;
                }
                else if (child is TextBlock textBlock)
                {
                    var defaultBrush = (Brush)Application.Current.Resources["DefaultTextForegroundThemeBrush"];                    
                    var disabledBrush = (Brush)Application.Current.Resources["ButtonForegroundDisabled"];
                    textBlock.Foreground = bEnabled ? defaultBrush : disabledBrush;
                }
                if (child is Image image)
                {
                    image.Opacity = bEnabled ? 1 : 0.5;
                    image.IsHitTestVisible = bEnabled ? true : false;
                }
                else if (child is StackPanel nestedPanel)
                {
                    EnableChildControls(nestedPanel, bEnabled);
                }
            }
        }               

        private async Task ResetEffectsAsync()
        {
            if (m_MediaCapture.CameraStreamState == CameraStreamState.Streaming)
            {
                await m_MediaCapture.ClearEffectsAsync(MediaStreamType.VideoPreview);
            }
        }

        private void cmbRotation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ((ComboBox)sender).SelectedItem;
            var sItem = ((Microsoft.UI.Xaml.Controls.ContentControl)item).Content.ToString();
            if (sItem == "90 Degrees")
            {
                m_Rotation = MediaRotation.Clockwise90Degrees;
            }
            else if (sItem == "180 Degrees")
            {
                m_Rotation = MediaRotation.Clockwise180Degrees;
            }
            else if (sItem == "270 Degrees")
            {
                m_Rotation = MediaRotation.Clockwise270Degrees;
            }
        }

        // Brightness
        public event PropertyChangedEventHandler PropertyChanged;
        private float _BrightnessWhitePointX = 1.0f;
        private float BrightnessWhitePointX
        {
            get => _BrightnessWhitePointX;
            set
            {
                _BrightnessWhitePointX = value;                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrightnessWhitePointX)));
            }
        }
        public double GetBrightnessWhitePointX(float? x) => _BrightnessWhitePointX;
        public float? SetBrightnessWhitePointX(double x) => BrightnessWhitePointX = (float)x;

        private float _BrightnessWhitePointY = 1.0f;
        private float BrightnessWhitePointY
        {
            get => _BrightnessWhitePointY;
            set
            {
                _BrightnessWhitePointY = value;                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrightnessWhitePointY)));
            }
        }
        public double GetBrightnessWhitePointY(float? x) => _BrightnessWhitePointY;
        public float? SetBrightnessWhitePointY(double x) => BrightnessWhitePointY = (float)x;

        private float _BrightnessBlackPointX = 0.0f;
        private float BrightnessBlackPointX
        {
            get => _BrightnessBlackPointX;
            set
            {
                _BrightnessBlackPointX = value;                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrightnessBlackPointX)));
            }
        }
        public double GetBrightnessBlackPointX(float? x) => _BrightnessBlackPointX;
        public float? SetBrightnessBlackPointX(double x) => BrightnessBlackPointX = (float)x;

        private float _BrightnessBlackPointY = 0.0f;
        private float BrightnessBlackPointY
        {
            get => _BrightnessBlackPointY;
            set
            {
                _BrightnessBlackPointY = value;                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BrightnessBlackPointY)));
            }
        }
        public double GetBrightnessBlackPointY(float? x) => _BrightnessBlackPointY;
        public float? SetBrightnessBlackPointY(double x) => BrightnessBlackPointY = (float)x;

        // Vignette
        private float _VignetteTransitionSize = 0.5f;
        private float VignetteTransitionSize
        {
            get => _VignetteTransitionSize;
            set
            {
                _VignetteTransitionSize = value;                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VignetteTransitionSize)));
            }
        }
        public double GetVignetteTransitionSize(float? x) => _VignetteTransitionSize;
        public float? SetVignetteTransitionSize(double x) => VignetteTransitionSize = (float)x;

        private float _VignetteStrength = 0.5f;
        private float VignetteStrength
        {
            get => _VignetteStrength;
            set
            {
                _VignetteStrength = value;                
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VignetteStrength)));
            }
        }
        public double GetVignetteStrength(float? x) => _VignetteStrength;
        public float? SetVignetteStrength(double x) => VignetteStrength = (float)x;

        private async void btn_RecordVideo_Click(object sender, RoutedEventArgs e)
        {
            if (tbFileVideo.Text == "")
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("File name to record the video must be filled", "Information");
                WinRT.Interop.InitializeWithWindow.Initialize(md, hWndMain);
                _ = await md.ShowAsync();
            }
            else
            {
                RecordVideo();
            }
        }

        public async Task<string> GetAudioCaptureDevicesAsync()
        {
            var devices = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);
            foreach (var device in devices)
            {
                Console.WriteLine($"Name: {device.Name}, ID: {device.Id}");
            }
            return devices.Count > 0 ? devices[0].Id : null; // Return the ID of the first device
        } 

        private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1"); // MF_MT_VIDEO_ROTATION in Mfapi.h

        private async void StopRecordVideo(bool bEnableControls)
        {
            if (m_MediaCapture != null)
            {
                if (m_bRecording)
                    await m_MediaCapture.StopRecordAsync();

                await m_MediaCapture.ClearEffectsAsync(MediaStreamType.VideoRecord);

                if (m_MirrorEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_MirrorEffectRecord);
                    m_MirrorEffectRecord = null;
                }
                //SafeRelease(ref m_pD2DBitmap1Effect);

                if (m_EmbossD2DEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_EmbossD2DEffectRecord);
                    m_EmbossD2DEffectRecord = null;
                }

                if (m_EdgeDetectionD2DEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_EdgeDetectionD2DEffectRecord);
                    m_EdgeDetectionD2DEffectRecord = null;                   
                }

                if (m_GaussianBlurD2DEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_GaussianBlurD2DEffectRecord);
                    m_GaussianBlurD2DEffectRecord = null;
                }

                if (m_SharpenD2DEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_SharpenD2DEffectRecord);
                    m_SharpenD2DEffectRecord = null;
                }

                if (m_VignetteD2DEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_VignetteD2DEffectRecord);
                    m_VignetteD2DEffectRecord = null;
                }

                if (m_RotationEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_RotationEffectRecord);
                    m_RotationEffectRecord = null;
                }

                if (m_GrayScaleD2DEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_GrayScaleD2DEffectRecord);
                    m_GrayScaleD2DEffectRecord = null;
                }

                if (m_RGBD2DEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_RGBD2DEffectRecord);
                    m_RGBD2DEffectRecord = null;
                }

                if (m_InvertD2DEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_InvertD2DEffectRecord);
                    m_InvertD2DEffectRecord = null;
                }

                if (m_BrightnessEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_BrightnessEffectRecord);
                    m_BrightnessEffectRecord = null;
                }              

                if (m_OverlayImageEffectRecord != null)
                {
                    //await m_MediaCapture.RemoveEffectAsync(m_OverlayImageEffectRecord);
                    m_OverlayImageEffectRecord = null;
                }

                m_bRecording = false;
                if (bEnableControls)
                {
                    bool isQueued = this.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        fi_RecordVideo.Glyph = "\U0001F3A5";
                        ttip_RecordVideo.Content = "Start recording video";
                        btnBrowseVideo.IsEnabled = true;
                        tbFileVideo.IsEnabled = true;
                        if (!m_bPreviewing)
                        {
                            cmbDevices.IsEnabled = true;
                            cmbFrameFormats.IsEnabled = true;
                            var defaultBrush = (Brush)Application.Current.Resources["DefaultTextForegroundThemeBrush"];
                            tbDevices.Foreground = defaultBrush;
                            tbFrameFormats.Foreground = defaultBrush;
                            EnableChildControls(sp_Effects, true);
                        }
                    });
                }
            }
        }

        private async void RecordVideo()
        {
            if (!m_bRecording)
            {
                if (m_frameSource != null)
                {
                    if (m_currentFrameFormat != null)
                    {
                        if (!((m_MediaCapture.MediaCaptureSettings.VideoDeviceCharacteristic == VideoDeviceCharacteristic.AllStreamsIdentical ||
                              m_MediaCapture.MediaCaptureSettings.VideoDeviceCharacteristic == VideoDeviceCharacteristic.PreviewRecordStreamsIdentical)
                              && m_bPreviewing))
                        {
                            if (cbOverlayImage.IsChecked == true)
                            {
                                var overlayImageEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.OverlayImageEffect");
                                m_OverlayImageEffectRecord = await m_MediaCapture.AddVideoEffectAsync(overlayImageEffectDefinition, MediaStreamType.VideoRecord);
                                if (m_OverlayImageScaled != null)
                                {
                                    m_OverlayImageScaled.Dispose();
                                    m_OverlayImageScaled = null;
                                }
                                if (m_OverlayImage.PixelWidth > m_currentFrameFormat.VideoFormat.Width / 2)
                                {
                                    double nRatio = (double)m_OverlayImage.PixelWidth / (double)(m_currentFrameFormat.VideoFormat.Width / 2);
                                    m_OverlayImageScaled = await ScaleSoftwareBitmapAsync(m_OverlayImage, m_currentFrameFormat.VideoFormat.Width / 2, (uint)(m_OverlayImage.PixelHeight / nRatio));
                                }
                                else
                                {
                                    m_OverlayImageScaled = SoftwareBitmap.Copy(m_OverlayImage);
                                }
                                m_OverlayImageEffectRecord.SetProperties(new PropertySet() { { "OverlayImage", m_OverlayImageScaled } });
                            }

                            if (tsMirror.IsOn)
                            {
                                var mirrorEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.MirrorEffect");
                                m_MirrorEffectRecord = await m_MediaCapture.AddVideoEffectAsync(mirrorEffectDefinition, MediaStreamType.VideoRecord);
                                m_MirrorEffectRecord.SetProperties(new PropertySet() { { "IsVertical", (m_Mirror == MediaMirroringOptions.Horizontal) ? false : true } });
                            }

                            if (cbEmboss.IsChecked == true)
                            {
                                var embossD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.EmbossD2DEffect");
                                m_EmbossD2DEffectRecord = await m_MediaCapture.AddVideoEffectAsync(embossD2DEffectDefinition, MediaStreamType.VideoRecord);                                
                                m_EmbossD2DEffectRecord.SetProperties(new PropertySet() { { "StrengthEmboss", m_StrengthEmboss } });
                            }

                            if (cbEdgeDetection.IsChecked == true)
                            {
                                var edgeDetectionD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.EdgeDetectionD2DEffect");
                                m_EdgeDetectionD2DEffectRecord = await m_MediaCapture.AddVideoEffectAsync(edgeDetectionD2DEffectDefinition, MediaStreamType.VideoRecord);                                
                                m_EdgeDetectionD2DEffectRecord.SetProperties(new PropertySet() { { "StrengthEdgeDetection", m_StrengthEdgeDetection } });
                            }

                            if (cbGaussianBlur.IsChecked == true)
                            {
                                var gaussianBlurD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.GaussianBlurD2DEffect");
                                m_GaussianBlurD2DEffectRecord = await m_MediaCapture.AddVideoEffectAsync(gaussianBlurD2DEffectDefinition, MediaStreamType.VideoRecord);
                                m_GaussianBlurD2DEffectRecord.SetProperties(new PropertySet() { { "DeviationGaussianBlur", m_DeviationGaussianBlur } });
                            }

                            if (cbSharpen.IsChecked == true)
                            {
                                var sharpenD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.SharpenD2DEffect");
                                m_SharpenD2DEffectRecord = await m_MediaCapture.AddVideoEffectAsync(sharpenD2DEffectDefinition, MediaStreamType.VideoRecord);
                                m_SharpenD2DEffectRecord.SetProperties(new PropertySet() { { "SharpnessSharpen", m_SharpnessSharpen } });
                            }

                            if (cbVignette.IsChecked == true)
                            {
                                var vignetteD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.VignetteD2DEffect");
                                m_VignetteD2DEffectRecord = await m_MediaCapture.AddVideoEffectAsync(vignetteD2DEffectDefinition, MediaStreamType.VideoRecord);
                                m_VignetteD2DEffectRecord.SetProperties(new PropertySet()
                                {
                                    { "VignetteStrength", _VignetteStrength },
                                    { "VignetteTransitionSize", _VignetteTransitionSize }                                   
                                });
                            }

                            if (tsRotation.IsOn)
                            {
                                // 0xc00d36b4 MF_E_INVALIDMEDIATYPE
                                // "Scaler unavaliable for type"
                                var rotationEffectDefinition = new VideoTransformEffectDefinition();
                                rotationEffectDefinition.Rotation = m_Rotation;
                                m_RotationEffectRecord = await m_MediaCapture.AddVideoEffectAsync(rotationEffectDefinition, MediaStreamType.VideoRecord);
                            }

                            if (cbGrayscale.IsChecked == true)
                            {
                                var grayScaleD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.GrayscaleD2DEffect");
                                m_GrayScaleD2DEffectRecord = await m_MediaCapture.AddVideoEffectAsync(grayScaleD2DEffectDefinition, MediaStreamType.VideoRecord);
                            }

                            if (cbRGB.IsChecked == true)
                            {
                                var rgbD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.RGBD2DEffect");
                                m_RGBD2DEffectRecord = await m_MediaCapture.AddVideoEffectAsync(rgbD2DEffectDefinition, MediaStreamType.VideoRecord);
                                m_RGBD2DEffectRecord.SetProperties(new PropertySet() { { "RedValue", m_Red }, { "GreenValue", m_Green }, { "BlueValue", m_Blue } });
                            }

                            if (cbInvert.IsChecked == true)
                            {
                                var invertD2DEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.InvertD2DEffect");
                                m_InvertD2DEffectRecord = await m_MediaCapture.AddVideoEffectAsync(invertD2DEffectDefinition, MediaStreamType.VideoRecord);
                            }

                            if (cbBrightness.IsChecked == true)
                            {
                                var brightnessEffectDefinition = new VideoEffectDefinition("VideoEffectComponent.LuminosityD2DEffect");
                                m_BrightnessEffectRecord = await m_MediaCapture.AddVideoEffectAsync(brightnessEffectDefinition, MediaStreamType.VideoRecord);                               
                                m_BrightnessEffectRecord.SetProperties(new PropertySet() 
                                {
                                    { "BrightnessWhitePointX", _BrightnessWhitePointX },
                                    { "BrightnessWhitePointY", _BrightnessWhitePointY }, 
                                    { "BrightnessBlackPointX", _BrightnessBlackPointX },
                                    { "BrightnessBlackPointY", _BrightnessBlackPointY }
                                });
                            }
                        }              

                        var encodingProfile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);

                        // Ensure Audio Encoding Properties are set
                        ////if (profile.Audio == null)
                        {
                            var audioProps = AudioEncodingProperties.CreateAac(44100, 2, 128000);
                            //    //var audioProps = AudioEncodingProperties.CreateAac(48000, 2, 192000);
                            encodingProfile.Audio = audioProps;
                        }
                        //encodingProfile.Video.Properties.Add(RotationKey, PropertyValue.CreateInt32(90));

                        m_MediaCapture.AudioDeviceController.VolumePercent = 100; // Max volume

                        //var myVideos = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Videos);
                        //using (System.IO.File.Create(tbFileVideo.Text)) { };
                        //StorageFile storageFile = await StorageFile.GetFileFromPathAsync(tbFileVideo.Text);

                        StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(tbFileVideo.Text));
                        StorageFile storageFile = await folder.CreateFileAsync(System.IO.Path.GetFileName(tbFileVideo.Text), CreationCollisionOption.ReplaceExisting);

                        //StorageFile storageFile = await folder.CreateFileAsync("photo_" + DateTime.Now.Ticks.ToString() + ".jpg", CreationCollisionOption.ReplaceExisting);
                        //m_mediaRecording = await m_MediaCapture.PrepareLowLagRecordToStorageFileAsync(encodingProfile, storageFile);
                        //await m_mediaRecording.StartAsync();
                        await m_MediaCapture.StartRecordToStorageFileAsync(encodingProfile, storageFile);

                        cmbDevices.IsEnabled = false;
                        cmbFrameFormats.IsEnabled = false;
                        var disabledBrush = (Brush)Application.Current.Resources["ButtonForegroundDisabled"];
                        tbDevices.Foreground = disabledBrush;
                        tbFrameFormats.Foreground = disabledBrush;
                        btnBrowseVideo.IsEnabled = false;
                        tbFileVideo.IsEnabled = false;
                        EnableChildControls(sp_Effects, false);
                        fi_RecordVideo.Glyph = "\u23F9";
                        ttip_RecordVideo.Content = "Stop recording video";
                        m_bRecording = true;
                    }
                    else
                    {
                        Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("No frame format selected", "Information");
                        WinRT.Interop.InitializeWithWindow.Initialize(md, hWndMain);
                        _ = await md.ShowAsync();
                    }
                }
                else
                {
                    Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("No device selected", "Information");
                    WinRT.Interop.InitializeWithWindow.Initialize(md, hWndMain);
                    _ = await md.ShowAsync();
                }
            }
            else
            {
                StopRecordVideo(true);               
            }
        }

        private async void btnBrowseVideo_Click(object sender, RoutedEventArgs e)
        {
            bool bRet = await RecordVideoDialogPicker();
        }

        private void tsMirror_Toggled(object sender, RoutedEventArgs e)
        {
        }

        private void tsRotation_Toggled(object sender, RoutedEventArgs e)
        {
        }

        private void tsHorizontal_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = sender as ToggleSwitch;
            m_Mirror = (ts.IsOn) ? MediaMirroringOptions.Horizontal : MediaMirroringOptions.Vertical;
        }

        private async Task<bool> RecordVideoDialogPicker()
        {
            try
            {
                var fsp = new Windows.Storage.Pickers.FileSavePicker();
                WinRT.Interop.InitializeWithWindow.Initialize(fsp, hWndMain);
                fsp.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                fsp.SuggestedFileName = "New Video";
                fsp.FileTypeChoices.Add("MP4 (H.264/AAC) (*.mp4)", new List<string>() { ".mp4" });
                fsp.FileTypeChoices.Add("Windows Media Video (*.wmv)", new List<string>() { ".wmv" });

                Windows.Storage.StorageFile file = await fsp.PickSaveFileAsync();
                if (file != null)
                {
                    tbFileVideo.Text = file.Path;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("Cannot save file " + tbFileVideo.Text + "\r\n" + "Exception : " + e.Message, "Error");
                WinRT.Interop.InitializeWithWindow.Initialize(md, hWndMain);
                _ = await md.ShowAsync();
                return false;
            }
        }

        private async void btnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            bool bRet = await SaveImageDialogPicker();
        }

        private async Task<bool> SaveImageDialogPicker()
        {
            var fsp = new Windows.Storage.Pickers.FileSavePicker();
            WinRT.Interop.InitializeWithWindow.Initialize(fsp, hWndMain);
            fsp.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            fsp.SuggestedFileName = "New Image";

            fsp.FileTypeChoices.Add("JPG Joint Photographic Experts Group (*.jpg)", new List<string>() { ".jpg" });
            fsp.FileTypeChoices.Add("PNG Portable Network Graphics (*.png)", new List<string>() { ".png" });
            fsp.FileTypeChoices.Add("GIF Graphics Interchange Format (*.gif)", new List<string>() { ".gif" });
            fsp.FileTypeChoices.Add("BMP Windows Bitmap (*.bmp)", new List<string>() { ".bmp" });
            fsp.FileTypeChoices.Add("TIF Tagged Image File Format (*.tif)", new List<string>() { ".tif" });
            fsp.FileTypeChoices.Add("HEIF High Efficiency Image File) (*.heif)", new List<string>() { ".heif" });

            Windows.Storage.StorageFile file = await fsp.PickSaveFileAsync();
            if (file != null)
            {
                tbFileImage.Text = file.Path;
                return true;
            }
            else
                return false;
        }

        private async void btn_SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (tbFileImage.Text == "")
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("File name to save a frame must be filled", "Information");
                WinRT.Interop.InitializeWithWindow.Initialize(md, hWndMain);
                _ = await md.ShowAsync();
            }
            else
            {
                if (m_currentFrameFormat != null)
                {
                    CapturePhotoToPathAsync(tbFileImage.Text);
                }
                else
                {
                    Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("No frame format selected", "Information");
                    WinRT.Interop.InitializeWithWindow.Initialize(md, hWndMain);
                    _ = await md.ShowAsync();
                }
            }
        }

        private Guid GetEncoderIdFromExtension(string extension)
        {
            Dictionary<string, Guid> encoderMap = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase)
            {
                { ".png", BitmapEncoder.PngEncoderId },
                { ".jpg", BitmapEncoder.JpegEncoderId },
                { ".jpeg", BitmapEncoder.JpegEncoderId },
                { ".bmp", BitmapEncoder.BmpEncoderId },
                { ".tiff", BitmapEncoder.TiffEncoderId },
                { ".gif", BitmapEncoder.GifEncoderId },
                { ".heif", BitmapEncoder.HeifEncoderId }
            };
            return encoderMap.TryGetValue(extension, out Guid encoderId) ? encoderId : BitmapEncoder.PngEncoderId; // Default to PNG
        }

        private async void CapturePhotoToPathAsync(string sFilePath)
        {
            try
            {
                StorageFile photoFile;
                try
                {
                    photoFile = await StorageFile.GetFileFromPathAsync(sFilePath);
                }
                catch
                {
                    // If file doesn't exist, create it
                    StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(System.IO.Path.GetDirectoryName(sFilePath));
                    photoFile = await folder.CreateFileAsync(System.IO.Path.GetFileName(sFilePath), CreationCollisionOption.ReplaceExisting);
                }

                using (IRandomAccessStream captureStream = new InMemoryRandomAccessStream())
                {                   
                    await m_MediaCapture.CapturePhotoToStreamAsync(Windows.Media.MediaProperties.ImageEncodingProperties.CreatePng(), captureStream);
                    using (IRandomAccessStream fileStream = await photoFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(captureStream);
                        Guid encoderId = GetEncoderIdFromExtension(Path.GetExtension(sFilePath));
                        BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, fileStream);
                        SoftwareBitmap sb = await decoder.GetSoftwareBitmapAsync();
                        encoder.SetSoftwareBitmap(sb);
                        await encoder.FlushAsync();

                        if (sb.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || sb.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
                        {
                            sb = SoftwareBitmap.Convert(sb, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                        }
                        var bitmapSource = new SoftwareBitmapSource();
                        await bitmapSource.SetBitmapAsync(sb);
                        imgPhoto.Source = bitmapSource;
                    }
                }
                //System.Diagnostics.Debug.WriteLine("Photo saved to: " + sFilePath);
                m_MP.Play();
            }
            catch (Exception ex)
            {              
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("Cannot save photo " + sFilePath + "\r\n" + "Exception : " + ex.Message, "Error");
                WinRT.Interop.InitializeWithWindow.Initialize(md, hWndMain);
                _ = await md.ShowAsync();               
            }
        }

        public HRESULT CreateD2D1Factory()
        {
            HRESULT hr = HRESULT.S_OK;
            D2D1_FACTORY_OPTIONS options = new D2D1_FACTORY_OPTIONS();

            // Needs "Enable native code Debugging"
#if DEBUG
            options.debugLevel = D2D1_DEBUG_LEVEL.D2D1_DEBUG_LEVEL_INFORMATION;
#endif

            hr = D2DTools.D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED, ref D2DTools.CLSID_D2D1Factory, ref options, out m_pD2DFactory);
            m_pD2DFactory1 = (ID2D1Factory1)m_pD2DFactory;
            return hr;
        }

        public HRESULT CreateDeviceContext()
        {
            HRESULT hr = HRESULT.S_OK;
            uint creationFlags = (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT;

            // Needs "Enable native code Debugging"
#if DEBUG
            creationFlags |= (uint)D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_DEBUG;
#endif

            int[] aD3D_FEATURE_LEVEL = new int[] { (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
                (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_1, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_3,
                (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_2, (int)D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1};

            D3D_FEATURE_LEVEL featureLevel;
            hr = D2DTools.D3D11CreateDevice(null,    // specify null to use the default adapter
                D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE,
                IntPtr.Zero,
                creationFlags,      // optionally set debug and Direct2D compatibility flags
                aD3D_FEATURE_LEVEL, // list of feature levels this app can support
                (uint)aD3D_FEATURE_LEVEL.Length, // number of possible feature levels
                D2DTools.D3D11_SDK_VERSION,
                out m_pD3D11DevicePtr,    // returns the Direct3D device created
                out featureLevel,         // returns feature level of device created            
                out m_pD3D11DeviceContext // returns the device immediate context
            );
            if (hr == HRESULT.S_OK)
            {
                m_pDXGIDevice = Marshal.GetObjectForIUnknown(m_pD3D11DevicePtr) as IDXGIDevice1;
                if (m_pD2DFactory1 != null)
                {
                    ID2D1Device pD2DDevice = null;
                    hr = m_pD2DFactory1.CreateDevice(m_pDXGIDevice, out pD2DDevice);
                    if (hr == HRESULT.S_OK)
                    {
                        hr = pD2DDevice.CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS.D2D1_DEVICE_CONTEXT_OPTIONS_NONE, out m_pD2DDeviceContext);
                        if (m_pD2DDeviceContext != null)
                            m_pD2DDeviceContext5 = (ID2D1DeviceContext5)m_pD2DDeviceContext;
                        SafeRelease(ref pD2DDevice);
                    }
                }
                //Marshal.Release(m_pD3D11DevicePtr);
            }
            return hr;
        }

        HRESULT CreateSwapChain(IntPtr hWnd)
        {
            HRESULT hr = HRESULT.S_OK;
            DXGI_SWAP_CHAIN_DESC1 swapChainDesc = new DXGI_SWAP_CHAIN_DESC1();
            swapChainDesc.Width = 1;
            swapChainDesc.Height = 1;
            swapChainDesc.Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM; // this is the most common swapchain format
            swapChainDesc.Stereo = false;
            swapChainDesc.SampleDesc.Count = 1;                // don't use multi-sampling
            swapChainDesc.SampleDesc.Quality = 0;
            swapChainDesc.BufferUsage = D2DTools.DXGI_USAGE_RENDER_TARGET_OUTPUT;
            swapChainDesc.BufferCount = 2;                     // use double buffering to enable flip
            swapChainDesc.Scaling = (hWnd != IntPtr.Zero) ? DXGI_SCALING.DXGI_SCALING_NONE : DXGI_SCALING.DXGI_SCALING_STRETCH;
            swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL; // all apps must use this SwapEffect       
            swapChainDesc.Flags = 0;

            //swapChainDesc.Scaling = DXGI_SCALING.DXGI_SCALING_NONE;
            swapChainDesc.AlphaMode = DXGI_ALPHA_MODE.DXGI_ALPHA_MODE_PREMULTIPLIED;

            IDXGIAdapter pDXGIAdapter;
            hr = m_pDXGIDevice.GetAdapter(out pDXGIAdapter);
            if (hr == HRESULT.S_OK)
            {
                IntPtr pDXGIFactory2Ptr;
                hr = pDXGIAdapter.GetParent(typeof(IDXGIFactory2).GUID, out pDXGIFactory2Ptr);
                if (hr == HRESULT.S_OK)
                {
                    IDXGIFactory2 pDXGIFactory2 = Marshal.GetObjectForIUnknown(pDXGIFactory2Ptr) as IDXGIFactory2;
                    if (hWnd != IntPtr.Zero)
                        hr = pDXGIFactory2.CreateSwapChainForHwnd(m_pD3D11DevicePtr, hWnd, ref swapChainDesc, IntPtr.Zero, null, out m_pDXGISwapChain1);
                    else
                        hr = pDXGIFactory2.CreateSwapChainForComposition(m_pD3D11DevicePtr, ref swapChainDesc, null, out m_pDXGISwapChain1);

                    if (hr == HRESULT.S_OK)
                    {
                        hr = m_pDXGIDevice.SetMaximumFrameLatency(1);
                    }
                    GlobalTools.SafeRelease(ref pDXGIFactory2);
                    Marshal.Release(pDXGIFactory2Ptr);
                }
                GlobalTools.SafeRelease(ref pDXGIAdapter);
            }
            return hr;
        }

        HRESULT ConfigureSwapChain(IntPtr hWnd)
        {
            HRESULT hr = HRESULT.S_OK;
            
            //IntPtr pD3D11Texture2DPtr = IntPtr.Zero;
            //hr = m_pDXGISwapChain1.GetBuffer(0, typeof(ID3D11Texture2D).GUID, ref pD3D11Texture2DPtr);
            //m_pD3D11Texture2D = Marshal.GetObjectForIUnknown(pD3D11Texture2DPtr) as ID3D11Texture2D;

            D2D1_BITMAP_PROPERTIES1 bitmapProperties = new D2D1_BITMAP_PROPERTIES1();
            bitmapProperties.bitmapOptions = D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_TARGET | D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_CANNOT_DRAW;
            //bitmapProperties.pixelFormat = D2DTools.PixelFormat(DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_IGNORE);
            bitmapProperties.pixelFormat = D2DTools.PixelFormat(DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_PREMULTIPLIED);
            uint nDPI = GetDpiForWindow(hWnd);
            bitmapProperties.dpiX = nDPI;
            bitmapProperties.dpiY = nDPI;
            IntPtr pDXGISurfacePtr = IntPtr.Zero;
            hr = m_pDXGISwapChain1.GetBuffer(0, typeof(IDXGISurface).GUID, out pDXGISurfacePtr);
            if (hr == HRESULT.S_OK)
            {
                IDXGISurface pDXGISurface = Marshal.GetObjectForIUnknown(pDXGISurfacePtr) as IDXGISurface;
                hr = m_pD2DDeviceContext.CreateBitmapFromDxgiSurface(pDXGISurface, ref bitmapProperties, out m_pD2DTargetBitmap);
                if (hr == HRESULT.S_OK)
                {
                    m_pD2DDeviceContext.SetTarget(m_pD2DTargetBitmap);
                }
                SafeRelease(ref pDXGISurface);
                Marshal.Release(pDXGISurfacePtr);
            }
            return hr;
        }

        private void scp1_SizeChanged(object sender, SizeChangedEventArgs e)
        {            
            Resize(e.NewSize);
        }

        HRESULT Resize(Windows.Foundation.Size sz)
        {
            HRESULT hr = HRESULT.S_OK;
            if (m_pDXGISwapChain1 != null)
            {
                if (m_pD2DDeviceContext != null)
                    m_pD2DDeviceContext.SetTarget(null);

                if (m_pD2DTargetBitmap != null)
                    SafeRelease(ref m_pD2DTargetBitmap);

                // 0, 0 => HRESULT: 0x80070057 (E_INVALIDARG) if not CreateSwapChainForHwnd
                //hr = m_pDXGISwapChain1.ResizeBuffers(
                // 2,
                // 0,
                // 0,
                // DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
                // 0
                // );
                if (sz.Width != 0 && sz.Height != 0)
                {
                    hr = m_pDXGISwapChain1.ResizeBuffers(
                      2,
                      (uint)sz.Width,
                      (uint)sz.Height,
                      DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
                      0
                      );
                }
                ConfigureSwapChain(hWndMain);
            }
            return (hr);
        }

        HRESULT LoadBitmapFromFile(ID2D1DeviceContext3 pDeviceContext3, IWICImagingFactory pIWICFactory, string uri, uint destinationWidth,
            uint destinationHeight, out ID2D1Bitmap1 pD2DBitmap, out IWICBitmapSource pBitmapSource)
        {
            HRESULT hr = HRESULT.S_OK;
            pD2DBitmap = null;
            pBitmapSource = null;

            IWICBitmapDecoder pDecoder = null;
            IWICBitmapFrameDecode pSource = null;
            IWICFormatConverter pConverter = null;
            IWICBitmapScaler pScaler = null;

            hr = pIWICFactory.CreateDecoderFromFilename(uri, Guid.Empty, unchecked((int)GENERIC_READ), WICDecodeOptions.WICDecodeMetadataCacheOnLoad, out pDecoder);
            if (hr == HRESULT.S_OK)
            {
                hr = pDecoder.GetFrame(0, out pSource);
                if (hr == HRESULT.S_OK)
                {
                    hr = pIWICFactory.CreateFormatConverter(out pConverter);
                    if (hr == HRESULT.S_OK)
                    {
                        if (destinationWidth != 0 || destinationHeight != 0)
                        {
                            uint originalWidth, originalHeight;
                            hr = pSource.GetSize(out originalWidth, out originalHeight);
                            if (hr == HRESULT.S_OK)
                            {
                                if (destinationWidth == 0)
                                {
                                    float scalar = (float)(destinationHeight) / (float)(originalHeight);
                                    destinationWidth = (uint)(scalar * (float)(originalWidth));
                                }
                                else if (destinationHeight == 0)
                                {
                                    float scalar = (float)(destinationWidth) / (float)(originalWidth);
                                    destinationHeight = (uint)(scalar * (float)(originalHeight));
                                }
                                hr = pIWICFactory.CreateBitmapScaler(out pScaler);
                                if (hr == HRESULT.S_OK)
                                {
                                    hr = pScaler.Initialize(pSource, destinationWidth, destinationHeight, WICBitmapInterpolationMode.WICBitmapInterpolationModeCubic);
                                    if (hr == HRESULT.S_OK)
                                    {
                                        hr = pConverter.Initialize(pScaler, GUID_WICPixelFormat32bppPBGRA, WICBitmapDitherType.WICBitmapDitherTypeNone, null, 0.0f, WICBitmapPaletteType.WICBitmapPaletteTypeMedianCut);
                                        //hr = pConverter.Initialize(pScaler, GUID_WICPixelFormat32bppBGRA, WICBitmapDitherType.WICBitmapDitherTypeNone, null, 0.0f, WICBitmapPaletteType.WICBitmapPaletteTypeMedianCut);
                                    }
                                    Marshal.ReleaseComObject(pScaler);
                                }
                            }
                        }
                        else // Don't scale the image.
                        {
                            hr = pConverter.Initialize(pSource, GUID_WICPixelFormat32bppPBGRA, WICBitmapDitherType.WICBitmapDitherTypeNone, null, 0.0f, WICBitmapPaletteType.WICBitmapPaletteTypeMedianCut);
                            //hr = pConverter.Initialize(pSource, GUID_WICPixelFormat32bppBGRA, WICBitmapDitherType.WICBitmapDitherTypeNone, null, 0.0f, WICBitmapPaletteType.WICBitmapPaletteTypeMedianCut);
                        }

                        // Create a Direct2D bitmap from the WIC bitmap.
                        D2D1_BITMAP_PROPERTIES1 bitmapProperties1 = new D2D1_BITMAP_PROPERTIES1();
                        bitmapProperties1.pixelFormat = D2DTools.PixelFormat(DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM, D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_PREMULTIPLIED);
                        bitmapProperties1.dpiX = 96;
                        bitmapProperties1.dpiY = 96;
                        hr = pDeviceContext3.CreateBitmapFromWicBitmap(pConverter, bitmapProperties1, out pD2DBitmap);

                        //if (pBitmapSource != null)
                        pBitmapSource = pConverter;
                    }
                    Marshal.ReleaseComObject(pSource);
                }
                Marshal.ReleaseComObject(pDecoder);
            }
            return hr;
        }

        HRESULT CreateDeviceResources()
        {
            HRESULT hr = HRESULT.S_OK;
            if (m_pD2DDeviceContext != null)
            {
                if (m_pD2DMainBrush == null)
                     hr = m_pD2DDeviceContext.CreateSolidColorBrush(new ColorF(ColorF.Enum.Black, 1.0f), BrushProperties(), out m_pD2DMainBrush);
                if (m_pD2DSolidColorBrushPink == null)
                    hr = m_pD2DDeviceContext.CreateSolidColorBrush(new ColorF(ColorF.Enum.DeepPink, 1.0f), BrushProperties(), out m_pD2DSolidColorBrushPink);

                string sExePath = AppContext.BaseDirectory;
                IWICBitmapSource pWICBitmapSource1 = null;
                string sAbsolutePath = "/Assets/webcam.jpg";
                if (sAbsolutePath.StartsWith("/"))
                    sAbsolutePath = sExePath + sAbsolutePath;
                hr = LoadBitmapFromFile(m_pD2DDeviceContext5, m_pWICImagingFactory, sAbsolutePath,
                    0, 0, out m_pD2DBitmap1, out pWICBitmapSource1);
                SafeRelease(ref pWICBitmapSource1);
            }
            return hr;
        }

        void CleanDeviceResources()
        {
            SafeRelease(ref m_pD2DBitmap1);
            SafeRelease(ref m_pD2DMainBrush);
            SafeRelease(ref m_pD2DSolidColorBrushPink);
        }

        void Clean()
        {
            //if (m_OverlayImage != null)
            //{
            //    m_OverlayImage.Dispose();
            //    m_OverlayImage = null;
            //}
            //if (m_OverlayImageScaled != null)
            //{
            //    m_OverlayImageScaled.Dispose();
            //    m_OverlayImageScaled = null;
            //}

            CleanDeviceResources();

            SafeRelease(ref m_pD2DTargetBitmap);
            SafeRelease(ref m_pDXGISwapChain1);

            SafeRelease(ref m_pD2DDeviceContext);

            SafeRelease(ref m_pD3D11DeviceContext);
            if (m_pD3D11DevicePtr != IntPtr.Zero)
                Marshal.Release(m_pD3D11DevicePtr);
            SafeRelease(ref m_pDXGIDevice);

            SafeRelease(ref m_pSharedTexture);

            SafeRelease(ref m_pWICImagingFactory);
            SafeRelease(ref m_pD2DFactory1);
            SafeRelease(ref m_pD2DFactory);
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            if (m_frameSource != null)
            {
                StopFrameReader(false, false);
                StopRecordVideo(false);
            }
            Clean();
        }
    }
}