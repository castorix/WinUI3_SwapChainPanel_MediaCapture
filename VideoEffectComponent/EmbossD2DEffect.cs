using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Media;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using WinRT;

using VideoEffectComponent.DirectXInternal;
using static VideoEffectComponent.DirectXInternal.DirectXInternalTools;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

//https://github.com/microsoft/CsWinRT/blob/master/docs/authoring.md#consuming-from-c-applications
//https://learn.microsoft.com/en-us/windows/uwp/audio-video-camera/custom-video-effects

namespace VideoEffectComponent
{
    public sealed class EmbossD2DEffect : IBasicVideoEffect
    {
        [ComImport]
        [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        unsafe interface IMemoryBufferByteAccess
        {
            void GetBuffer(out byte* buffer, out uint capacity);
        }

        [ComImport]
        [Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IDirect3DDxgiInterfaceAccess
        {
            HRESULT GetInterface([MarshalAs(UnmanagedType.LPStruct)] Guid iid, out IntPtr ppv);
        }

        // To avoid too many WindowsCreateStringReference()
        private float? m_StrengthEmboss = null;

        public bool IsReadOnly { get { return false; } }

        public IReadOnlyList<VideoEncodingProperties> SupportedEncodingProperties
        {
            get
            {
                var encodingProperties = new VideoEncodingProperties();
                encodingProperties.Subtype = "ARGB32";
                return new List<VideoEncodingProperties>() { encodingProperties };

                // If the list is empty, the encoding type will be ARGB32.
                // return new List<VideoEncodingProperties>();
            }
        }

        public MediaMemoryTypes SupportedMemoryTypes { get { return MediaMemoryTypes.Gpu; } }

        public bool TimeIndependent { get { return true; } }

        void IBasicVideoEffect.Close(MediaEffectClosedReason reason)
        {
            SafeRelease(ref m_pD2DDeviceContextEffect);
            SafeRelease(ref m_pD2DDeviceEffect);
            SafeRelease(ref m_pD2DFactoryEffect);
            SafeRelease(ref m_pD2DFactory1Effect);
        }

        private int nFrameCount;
        public void DiscardQueuedFrames()
        {
            nFrameCount = 0;
        }

        ID2D1Factory m_pD2DFactoryEffect = null;
        ID2D1Factory1 m_pD2DFactory1Effect = null;
        ID2D1Device m_pD2DDeviceEffect = null;
        ID2D1DeviceContext m_pD2DDeviceContextEffect = null;

        public unsafe void ProcessFrame(ProcessVideoFrameContext context)
        {
            HRESULT hr = HRESULT.S_OK;

            if (m_StrengthEmboss == null)
                m_StrengthEmboss = StrengthEmboss;

            var inputSurface = GetDXGISurface(context.InputFrame.Direct3DSurface);
            var outputSurface = GetDXGISurface(context.OutputFrame.Direct3DSurface);

            var inputBitmap = CreateBitmapFromSurface(inputSurface, D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_NONE);
            var renderTarget = CreateBitmapFromSurface(outputSurface, D2D1_BITMAP_OPTIONS.D2D1_BITMAP_OPTIONS_TARGET);

            m_pD2DDeviceContextEffect.GetSize(out D2D1_SIZE_F size);
            float nWidth = size.width;
            float nHeight = size.height;

            m_pD2DDeviceContextEffect.SetTarget(renderTarget);

            m_pD2DDeviceContextEffect.BeginDraw();

            //m_pD2DDeviceContextEffect.Clear(new ColorF(ColorF.Enum.Orange, 1.0f));

            D2D1_POINT_2F pt = new D2D1_POINT_2F(0, 0);
            D2D1_RECT_F destRect;
            destRect = new D2D1_RECT_F(0.0f, 0.0f, nWidth, nHeight);

            ID2D1Effect pEffect = null;
            hr = m_pD2DDeviceContextEffect.CreateEffect(CLSID_D2D1Emboss, out pEffect);
            if (hr == HRESULT.S_OK)
            {
                pEffect.SetInput(0, inputBitmap);
                SetEffectFloat(pEffect, (uint)D2D1_EMBOSS_PROP.D2D1_EMBOSS_PROP_HEIGHT, (float)m_StrengthEmboss);
                SetEffectFloat(pEffect, (uint)D2D1_EMBOSS_PROP.D2D1_EMBOSS_PROP_DIRECTION, 0.0f);
                ID2D1Image pOutputImage = null;
                pEffect.GetOutput(out pOutputImage);
                m_pD2DDeviceContextEffect.DrawImage(pOutputImage, ref pt, ref destRect, D2D1_INTERPOLATION_MODE.D2D1_INTERPOLATION_MODE_LINEAR, D2D1_COMPOSITE_MODE.D2D1_COMPOSITE_MODE_SOURCE_OVER);

                SafeRelease(ref pEffect);
                SafeRelease(ref pOutputImage);
            }

            //m_pD2DDeviceContextEffect.DrawImage(inputBitmap, ref pt, ref destRect, D2D1_INTERPOLATION_MODE.D2D1_INTERPOLATION_MODE_LINEAR, D2D1_COMPOSITE_MODE.D2D1_COMPOSITE_MODE_SOURCE_OVER);

            hr = m_pD2DDeviceContextEffect.EndDraw(out ulong tag1, out ulong tag2);

            SafeRelease(ref inputSurface);
            SafeRelease(ref outputSurface);
            SafeRelease(ref inputBitmap);
            SafeRelease(ref renderTarget);
        }

        private ID2D1Bitmap1 CreateBitmapFromSurface(IDXGISurface pSurface, D2D1_BITMAP_OPTIONS options)
        {
            uint nDPI = GetDpiForWindow(FindWindow("Shell_TrayWnd", null));
            var props = new D2D1_BITMAP_PROPERTIES1
            {
                pixelFormat = new D2D1_PIXEL_FORMAT
                {
                    format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
                    alphaMode = D2D1_ALPHA_MODE.D2D1_ALPHA_MODE_PREMULTIPLIED
                },
                //dpiX = 96,
                //dpiY = 96,
                dpiX = nDPI,
                dpiY = nDPI,
                bitmapOptions = options
            };
            HRESULT hr = m_pD2DDeviceContextEffect.CreateBitmapFromDxgiSurface(pSurface, ref props, out ID2D1Bitmap1 pD2DBitmap1);
            return pD2DBitmap1;
        }

        private IPropertySet configuration;
        public void SetProperties(IPropertySet configuration)
        {
            this.configuration = configuration;
        }

        private VideoEncodingProperties encodingProperties;
        public void SetEncodingProperties(VideoEncodingProperties encodingProperties, IDirect3DDevice device)
        {
            this.encodingProperties = encodingProperties;

            var pD3D11Device = GetD3D11Device(device);
            var pDXGIDevice = (IDXGIDevice)pD3D11Device;
            D2D1_FACTORY_OPTIONS options = new D2D1_FACTORY_OPTIONS();

            // Needs "Enable native code Debugging"            
#if DEBUG
            options.debugLevel = D2D1_DEBUG_LEVEL.D2D1_DEBUG_LEVEL_INFORMATION;
#endif

            HRESULT hr = HRESULT.S_OK;
            if (m_pD2DFactoryEffect is null)
            {
                hr = D2D1CreateFactory(D2D1_FACTORY_TYPE.D2D1_FACTORY_TYPE_MULTI_THREADED, ref CLSID_D2D1Factory, ref options, out m_pD2DFactoryEffect);
                m_pD2DFactory1Effect = (ID2D1Factory1)m_pD2DFactoryEffect;
            }
            if (m_pD2DDeviceEffect == null)
                hr = m_pD2DFactory1Effect.CreateDevice(pDXGIDevice, out m_pD2DDeviceEffect);
            if (hr == HRESULT.S_OK)
            {
                if (m_pD2DDeviceContextEffect == null)
                    hr = m_pD2DDeviceEffect.CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS.D2D1_DEVICE_CONTEXT_OPTIONS_NONE, out m_pD2DDeviceContextEffect);
            }
        }

        private ID3D11Device GetD3D11Device(IDirect3DDevice pDevice)
        {
            ID3D11Device pD3D11Device = null;
            IDirect3DDxgiInterfaceAccess dia = pDevice.As<IDirect3DDxgiInterfaceAccess>();
            IntPtr pDevicePtr = IntPtr.Zero;
            Guid guid = typeof(ID3D11Device).GUID;
            HRESULT hr = dia.GetInterface(guid, out pDevicePtr);
            if (hr == HRESULT.S_OK)
            {
                pD3D11Device = Marshal.GetObjectForIUnknown(pDevicePtr) as ID3D11Device;
                Marshal.Release(pDevicePtr);
            }
            return pD3D11Device;
        }

        private IDXGISurface GetDXGISurface(IDirect3DSurface pSurface)
        {
            IDXGISurface pDXGISurface = null;
            IDirect3DDxgiInterfaceAccess dia = pSurface.As<IDirect3DDxgiInterfaceAccess>();
            IntPtr pSurfacePtr = IntPtr.Zero;
            Guid guid = typeof(IDXGISurface).GUID;
            HRESULT hr = dia.GetInterface(guid, out pSurfacePtr);
            if (hr == HRESULT.S_OK)
            {
                pDXGISurface = Marshal.GetObjectForIUnknown(pSurfacePtr) as IDXGISurface;
                Marshal.Release(pSurfacePtr);
            }
            return pDXGISurface;
        }

        public float StrengthEmboss
        {
            get
            {
                object val;
                if (configuration != null && configuration.TryGetValue("StrengthEmboss", out val))
                {
                    return (float)val;
                }
                return 5.0f;
            }
        }
    }
}
