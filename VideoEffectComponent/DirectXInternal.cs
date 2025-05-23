using System;
using System.Runtime.InteropServices;

namespace VideoEffectComponent.DirectXInternal
{
    internal enum HRESULT : int
    {
        S_OK = 0,
        S_FALSE = 1,
        E_NOTIMPL = unchecked((int)0x80004001),
        E_NOINTERFACE = unchecked((int)0x80004002),
        E_POINTER = unchecked((int)0x80004003),
        E_FAIL = unchecked((int)0x80004005),
        E_UNEXPECTED = unchecked((int)0x8000FFFF),
        E_OUTOFMEMORY = unchecked((int)0x8007000E),
        E_INVALIDARG = unchecked((int)0x80070057),
    }

    [ComImport]
    [Guid("aec22fb8-76f3-4639-9be0-28eb43a67a2e")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDXGIObject
    {
        HRESULT SetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid Name, uint DataSize, IntPtr pData);
        HRESULT SetPrivateDataInterface([MarshalAs(UnmanagedType.LPStruct)] Guid Name, IntPtr pUnknown);
        HRESULT GetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid Name, ref uint pDataSize, out IntPtr pData);
        HRESULT GetParent([MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppParent);
    }

    [ComImport]
    [Guid("3d3e0379-f9de-4d58-bb6c-18d62992f1a6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDXGIDeviceSubObject : IDXGIObject
    {
        #region <IDXGIObject>
        new HRESULT SetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid Name, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface([MarshalAs(UnmanagedType.LPStruct)] Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent([MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppParent);
        #endregion

        HRESULT GetDevice([MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppDevice);
    }


    [ComImport]
    [Guid("cafcb56c-6ac3-4889-bf47-9e23bbd260ec")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDXGISurface : IDXGIDeviceSubObject
    {
        #region <IDXGIDeviceSubObject>

        #region <IDXGIObject>
        new HRESULT SetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid Name, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface([MarshalAs(UnmanagedType.LPStruct)] Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent([MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppParent);
        #endregion

        new HRESULT GetDevice([MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppDevice);
        #endregion

        HRESULT GetDesc(out DXGI_SURFACE_DESC pDesc);
        [PreserveSig]
        HRESULT Map(out DXGI_MAPPED_RECT pLockedRect, uint MapFlags);
        [PreserveSig]
        HRESULT Unmap();
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DXGI_SURFACE_DESC
    {
        public uint Width;
        public uint Height;
        public DXGI_FORMAT Format;
        public DXGI_SAMPLE_DESC SampleDesc;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct DXGI_MAPPED_RECT
    {
        public int Pitch;
        public IntPtr pBits;
    };

    internal enum DXGI_FORMAT
    {
        DXGI_FORMAT_UNKNOWN = 0,
        DXGI_FORMAT_R32G32B32A32_TYPELESS = 1,
        DXGI_FORMAT_R32G32B32A32_FLOAT = 2,
        DXGI_FORMAT_R32G32B32A32_UINT = 3,
        DXGI_FORMAT_R32G32B32A32_SINT = 4,
        DXGI_FORMAT_R32G32B32_TYPELESS = 5,
        DXGI_FORMAT_R32G32B32_FLOAT = 6,
        DXGI_FORMAT_R32G32B32_UINT = 7,
        DXGI_FORMAT_R32G32B32_SINT = 8,
        DXGI_FORMAT_R16G16B16A16_TYPELESS = 9,
        DXGI_FORMAT_R16G16B16A16_FLOAT = 10,
        DXGI_FORMAT_R16G16B16A16_UNORM = 11,
        DXGI_FORMAT_R16G16B16A16_UINT = 12,
        DXGI_FORMAT_R16G16B16A16_SNORM = 13,
        DXGI_FORMAT_R16G16B16A16_SINT = 14,
        DXGI_FORMAT_R32G32_TYPELESS = 15,
        DXGI_FORMAT_R32G32_FLOAT = 16,
        DXGI_FORMAT_R32G32_UINT = 17,
        DXGI_FORMAT_R32G32_SINT = 18,
        DXGI_FORMAT_R32G8X24_TYPELESS = 19,
        DXGI_FORMAT_D32_FLOAT_S8X24_UINT = 20,
        DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS = 21,
        DXGI_FORMAT_X32_TYPELESS_G8X24_UINT = 22,
        DXGI_FORMAT_R10G10B10A2_TYPELESS = 23,
        DXGI_FORMAT_R10G10B10A2_UNORM = 24,
        DXGI_FORMAT_R10G10B10A2_UINT = 25,
        DXGI_FORMAT_R11G11B10_FLOAT = 26,
        DXGI_FORMAT_R8G8B8A8_TYPELESS = 27,
        DXGI_FORMAT_R8G8B8A8_UNORM = 28,
        DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 29,
        DXGI_FORMAT_R8G8B8A8_UINT = 30,
        DXGI_FORMAT_R8G8B8A8_SNORM = 31,
        DXGI_FORMAT_R8G8B8A8_SINT = 32,
        DXGI_FORMAT_R16G16_TYPELESS = 33,
        DXGI_FORMAT_R16G16_FLOAT = 34,
        DXGI_FORMAT_R16G16_UNORM = 35,
        DXGI_FORMAT_R16G16_UINT = 36,
        DXGI_FORMAT_R16G16_SNORM = 37,
        DXGI_FORMAT_R16G16_SINT = 38,
        DXGI_FORMAT_R32_TYPELESS = 39,
        DXGI_FORMAT_D32_FLOAT = 40,
        DXGI_FORMAT_R32_FLOAT = 41,
        DXGI_FORMAT_R32_UINT = 42,
        DXGI_FORMAT_R32_SINT = 43,
        DXGI_FORMAT_R24G8_TYPELESS = 44,
        DXGI_FORMAT_D24_UNORM_S8_UINT = 45,
        DXGI_FORMAT_R24_UNORM_X8_TYPELESS = 46,
        DXGI_FORMAT_X24_TYPELESS_G8_UINT = 47,
        DXGI_FORMAT_R8G8_TYPELESS = 48,
        DXGI_FORMAT_R8G8_UNORM = 49,
        DXGI_FORMAT_R8G8_UINT = 50,
        DXGI_FORMAT_R8G8_SNORM = 51,
        DXGI_FORMAT_R8G8_SINT = 52,
        DXGI_FORMAT_R16_TYPELESS = 53,
        DXGI_FORMAT_R16_FLOAT = 54,
        DXGI_FORMAT_D16_UNORM = 55,
        DXGI_FORMAT_R16_UNORM = 56,
        DXGI_FORMAT_R16_UINT = 57,
        DXGI_FORMAT_R16_SNORM = 58,
        DXGI_FORMAT_R16_SINT = 59,
        DXGI_FORMAT_R8_TYPELESS = 60,
        DXGI_FORMAT_R8_UNORM = 61,
        DXGI_FORMAT_R8_UINT = 62,
        DXGI_FORMAT_R8_SNORM = 63,
        DXGI_FORMAT_R8_SINT = 64,
        DXGI_FORMAT_A8_UNORM = 65,
        DXGI_FORMAT_R1_UNORM = 66,
        DXGI_FORMAT_R9G9B9E5_SHAREDEXP = 67,
        DXGI_FORMAT_R8G8_B8G8_UNORM = 68,
        DXGI_FORMAT_G8R8_G8B8_UNORM = 69,
        DXGI_FORMAT_BC1_TYPELESS = 70,
        DXGI_FORMAT_BC1_UNORM = 71,
        DXGI_FORMAT_BC1_UNORM_SRGB = 72,
        DXGI_FORMAT_BC2_TYPELESS = 73,
        DXGI_FORMAT_BC2_UNORM = 74,
        DXGI_FORMAT_BC2_UNORM_SRGB = 75,
        DXGI_FORMAT_BC3_TYPELESS = 76,
        DXGI_FORMAT_BC3_UNORM = 77,
        DXGI_FORMAT_BC3_UNORM_SRGB = 78,
        DXGI_FORMAT_BC4_TYPELESS = 79,
        DXGI_FORMAT_BC4_UNORM = 80,
        DXGI_FORMAT_BC4_SNORM = 81,
        DXGI_FORMAT_BC5_TYPELESS = 82,
        DXGI_FORMAT_BC5_UNORM = 83,
        DXGI_FORMAT_BC5_SNORM = 84,
        DXGI_FORMAT_B5G6R5_UNORM = 85,
        DXGI_FORMAT_B5G5R5A1_UNORM = 86,
        DXGI_FORMAT_B8G8R8A8_UNORM = 87,
        DXGI_FORMAT_B8G8R8X8_UNORM = 88,
        DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM = 89,
        DXGI_FORMAT_B8G8R8A8_TYPELESS = 90,
        DXGI_FORMAT_B8G8R8A8_UNORM_SRGB = 91,
        DXGI_FORMAT_B8G8R8X8_TYPELESS = 92,
        DXGI_FORMAT_B8G8R8X8_UNORM_SRGB = 93,
        DXGI_FORMAT_BC6H_TYPELESS = 94,
        DXGI_FORMAT_BC6H_UF16 = 95,
        DXGI_FORMAT_BC6H_SF16 = 96,
        DXGI_FORMAT_BC7_TYPELESS = 97,
        DXGI_FORMAT_BC7_UNORM = 98,
        DXGI_FORMAT_BC7_UNORM_SRGB = 99,
        DXGI_FORMAT_AYUV = 100,
        DXGI_FORMAT_Y410 = 101,
        DXGI_FORMAT_Y416 = 102,
        DXGI_FORMAT_NV12 = 103,
        DXGI_FORMAT_P010 = 104,
        DXGI_FORMAT_P016 = 105,
        DXGI_FORMAT_420_OPAQUE = 106,
        DXGI_FORMAT_YUY2 = 107,
        DXGI_FORMAT_Y210 = 108,
        DXGI_FORMAT_Y216 = 109,
        DXGI_FORMAT_NV11 = 110,
        DXGI_FORMAT_AI44 = 111,
        DXGI_FORMAT_IA44 = 112,
        DXGI_FORMAT_P8 = 113,
        DXGI_FORMAT_A8P8 = 114,
        DXGI_FORMAT_B4G4R4A4_UNORM = 115,
        DXGI_FORMAT_FORCE_UINT = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DXGI_SAMPLE_DESC
    {
        public uint Count;
        public uint Quality;
    };

    [ComImport]
    [Guid("54ec77fa-1377-44e6-8c32-88fd5f44c84c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDXGIDevice : IDXGIObject
    {
        #region IDXGIObject
        new HRESULT SetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid Name, uint DataSize, IntPtr pData);
        new HRESULT SetPrivateDataInterface([MarshalAs(UnmanagedType.LPStruct)] Guid Name, IntPtr pUnknown);
        new HRESULT GetPrivateData([MarshalAs(UnmanagedType.LPStruct)] Guid Name, ref uint pDataSize, out IntPtr pData);
        new HRESULT GetParent([MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppParent);
        #endregion

        //HRESULT GetAdapter(out IDXGIAdapter pAdapter);
        //HRESULT CreateSurface(DXGI_SURFACE_DESC pDesc, uint NumSurfaces, uint Usage, ref DXGI_SHARED_RESOURCE pSharedResource, out IDXGISurface ppSurface);
        //HRESULT QueryResourceResidency(IntPtr ppResources, out DXGI_RESIDENCY pResidencyStatus, uint NumResources);
        //HRESULT SetGPUThreadPriority(int Priority);
        //HRESULT GetGPUThreadPriority(out int pPriority);
    }


    [ComImport]
    [Guid("db6f6ddb-ac77-4e88-8253-819df9bbf140")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID3D11Device
    {
    }
    
    internal enum D2D1_FACTORY_TYPE : uint
    {
        //
        // The resulting factory and derived resources may only be invoked serially.
        // Reference counts on resources are interlocked, however, resource and render
        // target state is not protected from multi-threaded access.
        //
        D2D1_FACTORY_TYPE_SINGLE_THREADED = 0,
        //
        // The resulting factory may be invoked from multiple threads. Returned resources
        // use interlocked reference counting and their state is protected.
        //
        D2D1_FACTORY_TYPE_MULTI_THREADED = 1,
        D2D1_FACTORY_TYPE_FORCE_DWORD = 0xffffffff
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_FACTORY_OPTIONS
    {
        public D2D1_DEBUG_LEVEL debugLevel;
    }

    internal enum D2D1_DEBUG_LEVEL : uint
    {
        D2D1_DEBUG_LEVEL_NONE = 0,
        D2D1_DEBUG_LEVEL_ERROR = 1,
        D2D1_DEBUG_LEVEL_WARNING = 2,
        D2D1_DEBUG_LEVEL_INFORMATION = 3,
        D2D1_DEBUG_LEVEL_FORCE_DWORD = 0xffffffff
    }

    [ComImport]
    [Guid("06152247-6f50-465a-9245-118bfd3b6007")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Factory
    {
        [PreserveSig]
        HRESULT ReloadSystemMetrics();
        [PreserveSig]
        HRESULT GetDesktopDpi(out float dpiX, out float dpiY);
        [PreserveSig]
        HRESULT CreateRectangleGeometry(ref D2D1_RECT_F rectangle, out IntPtr rectangleGeometry);
        [PreserveSig]
        HRESULT CreateRoundedRectangleGeometry(ref D2D1_ROUNDED_RECT roundedRectangle, out IntPtr roundedRectangleGeometry);
        [PreserveSig]
        HRESULT CreateEllipseGeometry(ref D2D1_ELLIPSE ellipse, out IntPtr ellipseGeometry);
        [PreserveSig]
        HRESULT CreateGeometryGroup(D2D1_FILL_MODE fillMode, IntPtr geometries, uint geometriesCount, out IntPtr geometryGroup);
        [PreserveSig]
        HRESULT CreateTransformedGeometry(IntPtr sourceGeometry, D2D1_MATRIX_3X2_F transform, out IntPtr transformedGeometry);
        [PreserveSig]
        HRESULT CreatePathGeometry(out IntPtr pathGeometry);
        [PreserveSig]
        IntPtr CreateStrokeStyle(ref D2D1_STROKE_STYLE_PROPERTIES strokeStyleProperties, [MarshalAs(UnmanagedType.LPArray)] float[] dashes = null, uint dashesCount = 0);
        [PreserveSig]
        HRESULT CreateDrawingStateBlock(ref D2D1_DRAWING_STATE_DESCRIPTION drawingStateDescription, IntPtr textRenderingParams, out IntPtr drawingStateBlock);
        [PreserveSig]
        HRESULT CreateWicBitmapRenderTarget(IntPtr target, D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, out IntPtr renderTarget);
        [PreserveSig]
        HRESULT CreateHwndRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties, out IntPtr hwndRenderTarget);
        [PreserveSig]
        HRESULT CreateDxgiSurfaceRenderTarget(IntPtr dxgiSurface, ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref IntPtr renderTarget);
        [PreserveSig]
        HRESULT CreateDCRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref IntPtr dcRenderTarget);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_RECT_F
    {
        public float left;
        public float top;
        public float right;
        public float bottom;
        public D2D1_RECT_F(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_RECT_U
    {
        public uint left;
        public uint top;
        public uint right;
        public uint bottom;
        public D2D1_RECT_U(uint left, uint top, uint right, uint bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_POINT_2F
    {
        public float x;
        public float y;

        public D2D1_POINT_2F(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_POINT_2U
    {
        public uint x;
        public uint y;

        public D2D1_POINT_2U(uint x, uint y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_SIZE_F
    {
        public float width;
        public float height;

        public D2D1_SIZE_F(float width, float height)
        {
            this.width = width;
            this.height = height;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_SIZE_U
    {
        public uint width;
        public uint height;
        public D2D1_SIZE_U(uint width, uint height)
        {
            this.width = width;
            this.height = height;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_ROUNDED_RECT
    {
        public D2D1_RECT_F rect;
        public float radiusX;
        public float radiusY;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_ELLIPSE
    {
        public D2D1_POINT_2F point;
        public float radiusX;
        public float radiusY;
    }

    internal enum D2D1_FILL_MODE
    {
        D2D1_FILL_MODE_ALTERNATE = 0,
        D2D1_FILL_MODE_WINDING = 1,
        D2D1_FILL_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    internal class D2D1_MATRIX_3X2_F
    {
        [FieldOffset(0)]
        public float _11;
        [FieldOffset(4)]
        public float _12;
        [FieldOffset(8)]
        public float _21;
        [FieldOffset(12)]
        public float _22;
        [FieldOffset(16)]
        public float _31;
        [FieldOffset(20)]
        public float _32;
    }

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    internal class D2D1_MATRIX_4X4_F
    {
        [FieldOffset(0)]
        public float _11;
        [FieldOffset(4)]
        public float _12;
        [FieldOffset(8)]
        public float _13;
        [FieldOffset(12)]
        public float _14;

        [FieldOffset(16)]
        public float _21;
        [FieldOffset(20)]
        public float _22;
        [FieldOffset(24)]
        public float _23;
        [FieldOffset(28)]
        public float _24;

        [FieldOffset(32)]
        public float _31;
        [FieldOffset(36)]
        public float _32;
        [FieldOffset(40)]
        public float _33;
        [FieldOffset(44)]
        public float _34;

        [FieldOffset(48)]
        public float _41;
        [FieldOffset(52)]
        public float _42;
        [FieldOffset(56)]
        public float _43;
        [FieldOffset(60)]
        public float _44;
    }

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    internal struct D2D1_MATRIX_3X2_F_STRUCT
    {
        [FieldOffset(0)]
        public float _11;
        [FieldOffset(4)]
        public float _12;
        [FieldOffset(8)]
        public float _21;
        [FieldOffset(12)]
        public float _22;
        [FieldOffset(16)]
        public float _31;
        [FieldOffset(20)]
        public float _32;
    }

    internal enum D2D1_CAP_STYLE : uint
    {
        /// <summary>
        /// Flat line cap.
        /// </summary>
        D2D1_CAP_STYLE_FLAT = 0,

        /// <summary>
        /// Square line cap.
        /// </summary>
        D2D1_CAP_STYLE_SQUARE = 1,

        /// <summary>
        /// Round line cap.
        /// </summary>
        D2D1_CAP_STYLE_ROUND = 2,

        /// <summary>
        /// Triangle line cap.
        /// </summary>
        D2D1_CAP_STYLE_TRIANGLE = 3,
        D2D1_CAP_STYLE_FORCE_DWORD = 0xffffffff

    }

    internal enum D2D1_DASH_STYLE
    {
        D2D1_DASH_STYLE_SOLID = 0,
        D2D1_DASH_STYLE_DASH = 1,
        D2D1_DASH_STYLE_DOT = 2,
        D2D1_DASH_STYLE_DASH_DOT = 3,
        D2D1_DASH_STYLE_DASH_DOT_DOT = 4,
        D2D1_DASH_STYLE_CUSTOM = 5,
        D2D1_DASH_STYLE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_LINE_JOIN
    {
        //
        // Miter join.
        //
        D2D1_LINE_JOIN_MITER = 0,
        //
        // Bevel join.
        //
        D2D1_LINE_JOIN_BEVEL = 1,
        //
        // Round join.
        //
        D2D1_LINE_JOIN_ROUND = 2,
        //
        // Miter/Bevel join.
        //
        D2D1_LINE_JOIN_MITER_OR_BEVEL = 3,
        D2D1_LINE_JOIN_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class D2D1_STROKE_STYLE_PROPERTIES
    {
        public D2D1_CAP_STYLE startCap;
        public D2D1_CAP_STYLE endCap;
        public D2D1_CAP_STYLE dashCap;
        public D2D1_LINE_JOIN lineJoin;
        public float miterLimit;
        public D2D1_DASH_STYLE dashStyle;
        public float dashOffset;
        public D2D1_STROKE_STYLE_PROPERTIES(D2D1_CAP_STYLE startCap, D2D1_CAP_STYLE endCap, D2D1_CAP_STYLE dashCap, D2D1_LINE_JOIN lineJoin, float miterLimit, D2D1_DASH_STYLE dashStyle, float dashOffset)
        {
            this.startCap = startCap;
            this.endCap = endCap;
            this.dashCap = dashCap;
            this.lineJoin = lineJoin;
            this.miterLimit = miterLimit;
            this.dashStyle = dashStyle;
            this.dashOffset = dashOffset;
        }
    }

    internal enum D2D1_ANTIALIAS_MODE
    {
        //
        // The edges of each primitive are antialiased sequentially.
        //
        D2D1_ANTIALIAS_MODE_PER_PRIMITIVE = 0,

        //
        // Each pixel is rendered if its pixel center is contained by the geometry.
        //
        D2D1_ANTIALIAS_MODE_ALIASED = 1,
        D2D1_ANTIALIAS_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }
    internal enum D2D1_TEXT_ANTIALIAS_MODE
    {
        //
        // Render text using the current system setting.
        //
        D2D1_TEXT_ANTIALIAS_MODE_DEFAULT = 0,
        //
        // Render text using ClearType.
        //
        D2D1_TEXT_ANTIALIAS_MODE_CLEARTYPE = 1,
        //
        // Render text using gray-scale.
        //
        D2D1_TEXT_ANTIALIAS_MODE_GRAYSCALE = 2,
        //
        // Render text aliased.
        //
        D2D1_TEXT_ANTIALIAS_MODE_ALIASED = 3,
        D2D1_TEXT_ANTIALIAS_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_DRAWING_STATE_DESCRIPTION
    {
        public D2D1_ANTIALIAS_MODE antialiasMode;
        public D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode;
        public UInt64 tag1;
        public UInt64 tag2;
        public D2D1_MATRIX_3X2_F transform;
    }

    internal enum D2D1_RENDER_TARGET_TYPE
    {
        //
        // D2D is free to choose the render target type for the caller.
        //
        D2D1_RENDER_TARGET_TYPE_DEFAULT = 0,
        //
        // The render target will render using the CPU.
        //
        D2D1_RENDER_TARGET_TYPE_SOFTWARE = 1,
        //
        // The render target will render using the GPU.
        //
        D2D1_RENDER_TARGET_TYPE_HARDWARE = 2,
        D2D1_RENDER_TARGET_TYPE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_ALPHA_MODE
    {
        //
        // Alpha mode should be determined implicitly. Some target surfaces do not supply
        // or imply this information in which case alpha must be specified.
        //
        D2D1_ALPHA_MODE_UNKNOWN = 0,
        //
        // Treat the alpha as premultipled.
        //
        D2D1_ALPHA_MODE_PREMULTIPLIED = 1,
        //
        // Opacity is in the 'A' component only.
        //
        D2D1_ALPHA_MODE_STRAIGHT = 2,
        //
        // Ignore any alpha channel information.
        //
        D2D1_ALPHA_MODE_IGNORE = 3,

        D2D1_ALPHA_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_PIXEL_FORMAT
    {
        public DXGI_FORMAT format;
        public D2D1_ALPHA_MODE alphaMode;
    }

    internal enum D2D1_RENDER_TARGET_USAGE
    {
        D2D1_RENDER_TARGET_USAGE_NONE = 0x00000000,
        //
        // Rendering will occur locally, if a terminal-services session is established, the
        // bitmap updates will be sent to the terminal services client.
        //
        D2D1_RENDER_TARGET_USAGE_FORCE_BITMAP_REMOTING = 0x00000001,
        //
        // The render target will allow a call to GetDC on the ID2D1GdiInteropRenderTarget
        // interface. Rendering will also occur locally.
        //
        D2D1_RENDER_TARGET_USAGE_GDI_COMPATIBLE = 0x00000002,
        D2D1_RENDER_TARGET_USAGE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D3D_FEATURE_LEVEL
    {
        D3D_FEATURE_LEVEL_1_0_CORE = 0x1000,
        D3D_FEATURE_LEVEL_9_1 = 0x9100,
        D3D_FEATURE_LEVEL_9_2 = 0x9200,
        D3D_FEATURE_LEVEL_9_3 = 0x9300,
        D3D_FEATURE_LEVEL_10_0 = 0xa000,
        D3D_FEATURE_LEVEL_10_1 = 0xa100,
        D3D_FEATURE_LEVEL_11_0 = 0xb000,
        D3D_FEATURE_LEVEL_11_1 = 0xb100,
        D3D_FEATURE_LEVEL_12_0 = 0xc000,
        D3D_FEATURE_LEVEL_12_1 = 0xc100
    }

    internal enum D2D1_FEATURE_LEVEL
    {
        //
        // The caller does not require a particular underlying D3D device level.
        //
        D2D1_FEATURE_LEVEL_DEFAULT = 0,

        //
        // The D3D device level is DX9 compatible.
        //
        D2D1_FEATURE_LEVEL_9 = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1,

        //
        // The D3D device level is DX10 compatible.
        //
        D2D1_FEATURE_LEVEL_10 = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0,
        D2D1_FEATURE_LEVEL_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_RENDER_TARGET_PROPERTIES
    {
        public D2D1_RENDER_TARGET_TYPE type;
        public D2D1_PIXEL_FORMAT pixelFormat;
        public float dpiX;
        public float dpiY;
        public D2D1_RENDER_TARGET_USAGE usage;
        public D2D1_FEATURE_LEVEL minLevel;
    }

    internal enum D2D1_PRESENT_OPTIONS
    {
        D2D1_PRESENT_OPTIONS_NONE = 0x00000000,
        //
        // Keep the target contents intact through present.
        //
        D2D1_PRESENT_OPTIONS_RETAIN_CONTENTS = 0x00000001,
        //
        // Do not wait for display refresh to commit changes to display.
        //
        D2D1_PRESENT_OPTIONS_IMMEDIATELY = 0x00000002,
        D2D1_PRESENT_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_HWND_RENDER_TARGET_PROPERTIES
    {
        public IntPtr hwnd;
        public D2D1_SIZE_U pixelSize;
        public D2D1_PRESENT_OPTIONS presentOptions;
    }

    [Guid("bb12d362-daee-4b9a-aa1d-14ba401cfa1f")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Factory1 : ID2D1Factory
    {
        #region <ID2D1Factory>
        [PreserveSig]
        new HRESULT ReloadSystemMetrics();
        [PreserveSig]
        new HRESULT GetDesktopDpi(out float dpiX, out float dpiY);
        [PreserveSig]
        new HRESULT CreateRectangleGeometry(ref D2D1_RECT_F rectangle, out IntPtr rectangleGeometry);
        [PreserveSig]
        new HRESULT CreateRoundedRectangleGeometry(ref D2D1_ROUNDED_RECT roundedRectangle, out IntPtr roundedRectangleGeometry);
        [PreserveSig]
        new HRESULT CreateEllipseGeometry(ref D2D1_ELLIPSE ellipse, out IntPtr ellipseGeometry);
        [PreserveSig]
        new HRESULT CreateGeometryGroup(D2D1_FILL_MODE fillMode, IntPtr geometries, uint geometriesCount, out IntPtr geometryGroup);
        [PreserveSig]
        new HRESULT CreateTransformedGeometry(IntPtr sourceGeometry, D2D1_MATRIX_3X2_F transform, out IntPtr transformedGeometry);
        [PreserveSig]
        new HRESULT CreatePathGeometry(out IntPtr pathGeometry);
        [PreserveSig]
        new IntPtr CreateStrokeStyle(ref D2D1_STROKE_STYLE_PROPERTIES strokeStyleProperties, [MarshalAs(UnmanagedType.LPArray)] float[] dashes = null, uint dashesCount = 0);
        [PreserveSig]
        new HRESULT CreateDrawingStateBlock(ref D2D1_DRAWING_STATE_DESCRIPTION drawingStateDescription, IntPtr textRenderingParams, out IntPtr drawingStateBlock);
        [PreserveSig]
        new HRESULT CreateWicBitmapRenderTarget(IntPtr target, D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, out IntPtr renderTarget);
        [PreserveSig]
        new HRESULT CreateHwndRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref D2D1_HWND_RENDER_TARGET_PROPERTIES hwndRenderTargetProperties, out IntPtr hwndRenderTarget);
        [PreserveSig]
        new HRESULT CreateDxgiSurfaceRenderTarget(IntPtr dxgiSurface, ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref IntPtr renderTarget);
        [PreserveSig]
        new HRESULT CreateDCRenderTarget(ref D2D1_RENDER_TARGET_PROPERTIES renderTargetProperties, ref IntPtr dcRenderTarget);
        #endregion

        [PreserveSig]
        HRESULT CreateDevice(IDXGIDevice dxgiDevice, out ID2D1Device d2dDevice);
        //    HRESULT CreateStrokeStyle(D2D1_STROKE_STYLE_PROPERTIES1 strokeStyleProperties, float dashes, uint dashesCount, out IntPtr strokeStyle);
        //    HRESULT CreatePathGeometry(out ID2D1PathGeometry1 pathGeometry);
        //    HRESULT CreateDrawingStateBlock(D2D1_DRAWING_STATE_DESCRIPTION1 drawingStateDescription, ref IntPtr textRenderingParams, out IntPtr drawingStateBlock);
        //    HRESULT CreateGdiMetafile(System.Runtime.InteropServices.ComTypes.IStream metafileStream, out IntPtr metafile);
    }

    [ComImport]
    [Guid("2cd90691-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Resource
    {
        [PreserveSig]
        void GetFactory(out ID2D1Factory factory);
    }

    internal enum D2D1_DEVICE_CONTEXT_OPTIONS
    {
        D2D1_DEVICE_CONTEXT_OPTIONS_NONE = 0,
        /// <summary>
        /// Geometry rendering will be performed on many threads in parallel, a single
        /// thread is the default.
        /// </summary>
        D2D1_DEVICE_CONTEXT_OPTIONS_ENABLE_MULTITHREADED_OPTIMIZATIONS = 1,
        D2D1_DEVICE_CONTEXT_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_PRINT_FONT_SUBSET_MODE
    {
        /// <summary>
        /// Subset for used glyphs, send and discard font resource after every five pages
        /// </summary>
        D2D1_PRINT_FONT_SUBSET_MODE_DEFAULT = 0,

        /// <summary>
        /// Subset for used glyphs, send and discard font resource after each page
        /// </summary>
        D2D1_PRINT_FONT_SUBSET_MODE_EACHPAGE = 1,

        /// <summary>
        /// Do not subset, reuse font for all pages, send it after first page
        /// </summary>
        D2D1_PRINT_FONT_SUBSET_MODE_NONE = 2,
        D2D1_PRINT_FONT_SUBSET_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_COLOR_SPACE
    {
        /// <summary>
        /// The color space is described by accompanying data, such as a color profile.
        /// </summary>
        D2D1_COLOR_SPACE_CUSTOM = 0,

        /// <summary>
        /// The sRGB color space.
        /// </summary>
        D2D1_COLOR_SPACE_SRGB = 1,

        /// <summary>
        /// The scRGB color space.
        /// </summary>
        D2D1_COLOR_SPACE_SCRGB = 2,
        D2D1_COLOR_SPACE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_PRINT_CONTROL_PROPERTIES
    {
        public D2D1_PRINT_FONT_SUBSET_MODE fontSubset;

        /// <summary>
        /// DPI for rasterization of all unsupported D2D commands or options, defaults to
        /// 150.0
        /// </summary>
        public float rasterDPI;

        /// <summary>
        /// Color space for vector graphics in XPS package
        /// </summary>
        public D2D1_COLOR_SPACE colorSpace;
    };

    [ComImport]
    [Guid("47dd575d-ac05-4cdd-8049-9b02cd16f44c")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Device : ID2D1Resource
    {
        #region <ID2D1Resource>
        [PreserveSig]
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        [PreserveSig]
        HRESULT CreateDeviceContext(D2D1_DEVICE_CONTEXT_OPTIONS options, out ID2D1DeviceContext deviceContext);
        //HRESULT CreatePrintControl(IWICImagingFactory wicFactory, IPrintDocumentPackageTarget documentTarget, D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out ID2D1PrintControl printControl);
        [PreserveSig]
        HRESULT CreatePrintControl(IntPtr wicFactory, IntPtr documentTarget, ref D2D1_PRINT_CONTROL_PROPERTIES printControlProperties, out IntPtr printControl);
        [PreserveSig]
        void SetMaximumTextureMemory(UInt64 maximumInBytes);
        [PreserveSig]
        UInt64 GetMaximumTextureMemory();
        [PreserveSig]
        void ClearResources(uint millisecondsSinceUse = 0);
    }

    [ComImport]
    [Guid("2cd90694-12e2-11dc-9fed-001143a055f9")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1RenderTarget : ID2D1Resource
    {
        #region <ID2D1Resource>

        [PreserveSig]
        new void GetFactory(out ID2D1Factory factory);

        #endregion
        HRESULT Function1();
        HRESULT Function2();
        HRESULT Function3();
        HRESULT Function4();
        HRESULT Function5();
        HRESULT Function6();
        HRESULT Function7();
        HRESULT Function8();
        HRESULT Function9();
        HRESULT Function10();
        void Function11();
        void Function12();
        void Function13();
        void Function14();
        void Function15();
        void Function16();
        void Function17();
        void Function18();
        void Function19();
        void Function20();
        void Function21();
        void Function22();
        void Function23();
        void Function24();
        void Function25();
        void Function26();
        [PreserveSig]
        void SetTransform(D2D1_MATRIX_3X2_F transform);
        [PreserveSig]
        void GetTransform(out D2D1_MATRIX_3X2_F_STRUCT transform);
        [PreserveSig]
        void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        [PreserveSig]
        void GetAntialiasMode(out D2D1_ANTIALIAS_MODE antialiasMode);        
        void Function31();
        [PreserveSig]
        void GetTextAntialiasMode(out D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        void Function33();
        void Function34();
        void Function35();
        void Function36();
        void Function37();
        void Function38();
        void Function39();
        void Function40();
        void Function41();
        void Function42();
        void Function43();
        [PreserveSig]
        void Clear(D2D1_COLOR_F clearColor);
        [PreserveSig]
        void BeginDraw();
        [PreserveSig]
        HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        [PreserveSig]
        void GetPixelFormat(out D2D1_PIXEL_FORMAT pixelFormat);
        [PreserveSig]
        void SetDpi(float dpiX, float dpiY);
        [PreserveSig]
        void GetDpi(out float dpiX, out float dpiY);
        [PreserveSig]
        void GetSize(out D2D1_SIZE_F size);
        [PreserveSig]
        void GetPixelSize(out D2D1_SIZE_U size);
        [PreserveSig]
        uint GetMaximumBitmapSize();
        [PreserveSig]
        bool Function53();
    }

    [ComImport]
    [Guid("e8f7fe7a-191c-466d-ad95-975678bda998")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1DeviceContext : ID2D1RenderTarget
    {
        #region <ID2D1RenderTarget>

        #region <ID2D1Resource>
        [PreserveSig]
        new void GetFactory(out ID2D1Factory factory);
        #endregion

        new HRESULT Function1();
        new HRESULT Function2();
        new HRESULT Function3();
        new HRESULT Function4();
        new HRESULT Function5();
        new HRESULT Function6();
        new HRESULT Function7();
        new HRESULT Function8();
        new HRESULT Function9();
        new HRESULT Function10();
        new void Function11();
        new void Function12();
        new void Function13();
        new void Function14();
        new void Function15();
        new void Function16();
        new void Function17();
        new void Function18();
        new void Function19();
        new void Function20();
        new void Function21();
        new void Function22();
        new void Function23();
        new void Function24();
        new void Function25();
        new void Function26();
        [PreserveSig]
        new void SetTransform(D2D1_MATRIX_3X2_F transform);
        [PreserveSig]
        new void GetTransform(out D2D1_MATRIX_3X2_F_STRUCT transform);
        [PreserveSig]
        new void SetAntialiasMode(D2D1_ANTIALIAS_MODE antialiasMode);
        [PreserveSig]
        new void GetAntialiasMode(out D2D1_ANTIALIAS_MODE antialiasMode);
        new void Function31();
        [PreserveSig]
        new void GetTextAntialiasMode(out D2D1_TEXT_ANTIALIAS_MODE textAntialiasMode);
        new void Function33();
        new void Function34();
        new void Function35();
        new void Function36();
        new void Function37();
        new void Function38();
        new void Function39();
        new void Function40();
        new void Function41();
        new void Function42();
        new void Function43();
        [PreserveSig]
        new void Clear(D2D1_COLOR_F clearColor);
        [PreserveSig]
        new void BeginDraw();
        [PreserveSig]
        new HRESULT EndDraw(out UInt64 tag1, out UInt64 tag2);
        [PreserveSig]
        new void GetPixelFormat(out D2D1_PIXEL_FORMAT pixelFormat);
        [PreserveSig]
        new void SetDpi(float dpiX, float dpiY);
        [PreserveSig]
        new void GetDpi(out float dpiX, out float dpiY);
        [PreserveSig]
        new void GetSize(out D2D1_SIZE_F size);
        [PreserveSig]
        new void GetPixelSize(out D2D1_SIZE_U size);
        [PreserveSig]
        new uint GetMaximumBitmapSize();
        [PreserveSig]
        new bool Function53();

        #endregion

        [PreserveSig]
        HRESULT CreateBitmap(D2D1_SIZE_U size, IntPtr sourceData, uint pitch, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        [PreserveSig]
        HRESULT CreateBitmapFromWicBitmap(IntPtr wicBitmapSource, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        [PreserveSig]
        HRESULT CreateColorContext(D2D1_COLOR_SPACE space, IntPtr profile, uint profileSize, out IntPtr colorContext);
        [PreserveSig]
        HRESULT CreateColorContextFromFilename(string filename, out IntPtr colorContext);
        [PreserveSig]
        HRESULT CreateColorContextFromWicColorContext(IntPtr wicColorContext, out IntPtr colorContext);
        [PreserveSig]
        HRESULT CreateBitmapFromDxgiSurface(IDXGISurface surface, ref D2D1_BITMAP_PROPERTIES1 bitmapProperties, out ID2D1Bitmap1 bitmap);
        [PreserveSig]
        HRESULT CreateEffect(ref Guid effectId, out ID2D1Effect effect);
        [PreserveSig]
        HRESULT CreateGradientStopCollection([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] D2D1_GRADIENT_STOP[] straightAlphaGradientStops, uint straightAlphaGradientStopsCount, D2D1_COLOR_SPACE preInterpolationSpace,
            D2D1_COLOR_SPACE postInterpolationSpace, D2D1_BUFFER_PRECISION bufferPrecision, D2D1_EXTEND_MODE extendMode, D2D1_COLOR_INTERPOLATION_MODE colorInterpolationMode,
            out IntPtr gradientStopCollection1);
        [PreserveSig]
        HRESULT CreateImageBrush(ID2D1Image image, ref D2D1_IMAGE_BRUSH_PROPERTIES imageBrushProperties, ref D2D1_BRUSH_PROPERTIES brushProperties, out IntPtr imageBrush);
        [PreserveSig]
        HRESULT CreateBitmapBrush(ID2D1Bitmap bitmap, ref D2D1_BITMAP_BRUSH_PROPERTIES1 bitmapBrushProperties, ref D2D1_BRUSH_PROPERTIES brushProperties, out IntPtr bitmapBrush);
        [PreserveSig]
        HRESULT CreateCommandList(out IntPtr commandList);
        [PreserveSig]
        bool IsDxgiFormatSupported(DXGI_FORMAT format);
        [PreserveSig]
        bool IsBufferPrecisionSupported(D2D1_BUFFER_PRECISION bufferPrecision);
        [PreserveSig]
        HRESULT GetImageLocalBounds(ID2D1Image image, out D2D1_RECT_F localBounds);
        [PreserveSig]
        HRESULT GetImageWorldBounds(ID2D1Image image, out D2D1_RECT_F worldBounds);
        [PreserveSig]
        HRESULT GetGlyphRunWorldBounds(ref D2D1_POINT_2F baselineOrigin, DWRITE_GLYPH_RUN glyphRun, DWRITE_MEASURING_MODE measuringMode, out D2D1_RECT_F bounds);
        [PreserveSig]
        void GetDevice(out ID2D1Device device);
        [PreserveSig]
        void SetTarget(ID2D1Image image);
        [PreserveSig]
        void GetTarget(out ID2D1Image image);
        [PreserveSig]
        void SetRenderingControls(D2D1_RENDERING_CONTROLS renderingControls);
        [PreserveSig]
        void GetRenderingControls(out D2D1_RENDERING_CONTROLS renderingControls);
        [PreserveSig]
        void SetPrimitiveBlend(D2D1_PRIMITIVE_BLEND primitiveBlend);
        [PreserveSig]
        void GetPrimitiveBlend(out D2D1_PRIMITIVE_BLEND primitiveBlend);
        [PreserveSig]
        void SetUnitMode(D2D1_UNIT_MODE unitMode);
        [PreserveSig]
        void GetUnitMode(out D2D1_UNIT_MODE unitMode);
        [PreserveSig]
        void DrawGlyphRun(D2D1_POINT_2F baselineOrigin, ref DWRITE_GLYPH_RUN glyphRun, IntPtr glyphRunDescription, IntPtr foregroundBrush, DWRITE_MEASURING_MODE measuringMode);
        [PreserveSig]
        void DrawImage(ID2D1Image image, ref D2D1_POINT_2F targetOffset, ref D2D1_RECT_F imageRectangle, D2D1_INTERPOLATION_MODE interpolationMode, D2D1_COMPOSITE_MODE compositeMode);
        [PreserveSig]
        void DrawGdiMetafile(IntPtr gdiMetafile, ref D2D1_POINT_2F targetOffset);
        [PreserveSig]
        void DrawBitmap(ID2D1Bitmap bitmap, ref D2D1_RECT_F destinationRectangle, float opacity, D2D1_INTERPOLATION_MODE interpolationMode, ref D2D1_RECT_F sourceRectangle, D2D1_MATRIX_4X4_F perspectiveTransform = null);
        [PreserveSig]
        void PushLayer(ref D2D1_LAYER_PARAMETERS1 layerParameters, IntPtr layer);
        [PreserveSig]
        HRESULT InvalidateEffectInputRectangle(ID2D1Effect effect, uint input, D2D1_RECT_F inputRectangle);
        [PreserveSig]
        HRESULT GetEffectInvalidRectangleCount(ID2D1Effect effect, out uint rectangleCount);
        //HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out D2D1_RECT_F* rectangles, uint rectanglesCount);
        [PreserveSig]
        HRESULT GetEffectInvalidRectangles(ID2D1Effect effect, out IntPtr rectangles, uint rectanglesCount);
        [PreserveSig]
        HRESULT GetEffectRequiredInputRectangles(ID2D1Effect renderEffect, D2D1_RECT_F renderImageRectangle, D2D1_EFFECT_INPUT_DESCRIPTION inputDescriptions,
            //out D2D1_RECT_F* requiredInputRects, uint inputCount);
            out IntPtr requiredInputRects, uint inputCount);
        [PreserveSig]
        void FillOpacityMask(ID2D1Bitmap opacityMask, IntPtr brush, ref D2D1_RECT_F destinationRectangle, D2D1_RECT_F sourceRectangle);
    }

    [ComImport]
    [Guid("483473d7-cd46-4f9d-9d3a-3112aa80159d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Properties
    {
        [PreserveSig]
        uint GetPropertyCount();
        [PreserveSig]
        HRESULT GetPropertyName(uint index, out string name, uint nameCount);
        [PreserveSig]
        uint GetPropertyNameLength(uint index);
        [PreserveSig]
        D2D1_PROPERTY_TYPE GetType(uint index);
        [PreserveSig]
        uint GetPropertyIndex(string name);
        [PreserveSig]
        HRESULT SetValueByName(string name, D2D1_PROPERTY_TYPE type, IntPtr data, uint dataSize);
        [PreserveSig]
        HRESULT SetValue(uint index, D2D1_PROPERTY_TYPE type, IntPtr data, uint dataSize);
        [PreserveSig]
        HRESULT GetValueByName(string name, D2D1_PROPERTY_TYPE type, out IntPtr data, uint dataSize);
        [PreserveSig]
        HRESULT GetValue(uint index, D2D1_PROPERTY_TYPE type, out IntPtr data, uint dataSize);
        [PreserveSig]
        uint GetValueSize(uint index);
        [PreserveSig]
        HRESULT GetSubProperties(uint index, out ID2D1Properties subProperties);
    }

    internal enum D2D1_PROPERTY_TYPE
    {
        D2D1_PROPERTY_TYPE_UNKNOWN = 0,
        D2D1_PROPERTY_TYPE_STRING = 1,
        D2D1_PROPERTY_TYPE_BOOL = 2,
        D2D1_PROPERTY_TYPE_UINT32 = 3,
        D2D1_PROPERTY_TYPE_INT32 = 4,
        D2D1_PROPERTY_TYPE_FLOAT = 5,
        D2D1_PROPERTY_TYPE_VECTOR2 = 6,
        D2D1_PROPERTY_TYPE_VECTOR3 = 7,
        D2D1_PROPERTY_TYPE_VECTOR4 = 8,
        D2D1_PROPERTY_TYPE_BLOB = 9,
        D2D1_PROPERTY_TYPE_IUNKNOWN = 10,
        D2D1_PROPERTY_TYPE_ENUM = 11,
        D2D1_PROPERTY_TYPE_ARRAY = 12,
        D2D1_PROPERTY_TYPE_CLSID = 13,
        D2D1_PROPERTY_TYPE_MATRIX_3X2 = 14,
        D2D1_PROPERTY_TYPE_MATRIX_4X3 = 15,
        D2D1_PROPERTY_TYPE_MATRIX_4X4 = 16,
        D2D1_PROPERTY_TYPE_MATRIX_5X4 = 17,
        D2D1_PROPERTY_TYPE_COLOR_CONTEXT = 18,
        D2D1_PROPERTY_TYPE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [ComImport]
    [Guid("28211a43-7d89-476f-8181-2d6159b220ad")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Effect : ID2D1Properties
    {
        #region <ID2D1Properties>
        [PreserveSig]
        new uint GetPropertyCount();
        [PreserveSig]
        new HRESULT GetPropertyName(uint index, out string name, uint nameCount);
        [PreserveSig]
        new uint GetPropertyNameLength(uint index);
        [PreserveSig]
        new D2D1_PROPERTY_TYPE GetType(uint index);
        [PreserveSig]
        new uint GetPropertyIndex(string name);
        [PreserveSig]
        new HRESULT SetValueByName(string name, D2D1_PROPERTY_TYPE type, IntPtr data, uint dataSize);
        [PreserveSig]
        new HRESULT SetValue(uint index, D2D1_PROPERTY_TYPE type, IntPtr data, uint dataSize);
        [PreserveSig]
        new HRESULT GetValueByName(string name, D2D1_PROPERTY_TYPE type, out IntPtr data, uint dataSize);
        [PreserveSig]
        new HRESULT GetValue(uint index, D2D1_PROPERTY_TYPE type, out IntPtr data, uint dataSize);
        [PreserveSig]
        new uint GetValueSize(uint index);
        [PreserveSig]
        new HRESULT GetSubProperties(uint index, out ID2D1Properties subProperties);
        #endregion

        [PreserveSig]
        void SetInput(uint index, ID2D1Image input, bool invalidate = true);
        [PreserveSig]
        HRESULT SetInputCount(uint inputCount);
        [PreserveSig]
        void GetInput(uint index, out ID2D1Image input);
        [PreserveSig]
        uint GetInputCount();
        [PreserveSig]
        void GetOutput(out ID2D1Image outputImage);
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class D2D1_COLOR_F
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_GRADIENT_STOP
    {
        public float position;
        public D2D1_COLOR_F color;

        public D2D1_GRADIENT_STOP(float position, D2D1_COLOR_F color)
        {
            this.position = position;
            this.color = color;
        }
    }

    internal enum D2D1_BUFFER_PRECISION
    {
        D2D1_BUFFER_PRECISION_UNKNOWN = 0,
        D2D1_BUFFER_PRECISION_8BPC_UNORM = 1,
        D2D1_BUFFER_PRECISION_8BPC_UNORM_SRGB = 2,
        D2D1_BUFFER_PRECISION_16BPC_UNORM = 3,
        D2D1_BUFFER_PRECISION_16BPC_FLOAT = 4,
        D2D1_BUFFER_PRECISION_32BPC_FLOAT = 5,
        D2D1_BUFFER_PRECISION_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_EXTEND_MODE
    {
        //
        // Extend the edges of the source out by clamping sample points outside the source
        // to the edges.
        //
        D2D1_EXTEND_MODE_CLAMP = 0,
        //
        // The base tile is drawn untransformed and the remainder are filled by repeating
        // the base tile.
        //
        D2D1_EXTEND_MODE_WRAP = 1,
        //
        // The same as wrap, but alternate tiles are flipped  The base tile is drawn
        // untransformed.
        //
        D2D1_EXTEND_MODE_MIRROR = 2,
        D2D1_EXTEND_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_COLOR_INTERPOLATION_MODE
    {
        /// <summary>
        /// Colors will be interpolated in straight alpha space.
        /// </summary>
        D2D1_COLOR_INTERPOLATION_MODE_STRAIGHT = 0,

        /// <summary>
        /// Colors will be interpolated in premultiplied alpha space.
        /// </summary>
        D2D1_COLOR_INTERPOLATION_MODE_PREMULTIPLIED = 1,
        D2D1_COLOR_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_PRIMITIVE_BLEND
    {
        D2D1_PRIMITIVE_BLEND_SOURCE_OVER = 0,
        D2D1_PRIMITIVE_BLEND_COPY = 1,
        D2D1_PRIMITIVE_BLEND_MIN = 2,
        D2D1_PRIMITIVE_BLEND_ADD = 3,
        D2D1_PRIMITIVE_BLEND_MAX = 4,
        D2D1_PRIMITIVE_BLEND_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_INTERPOLATION_MODE_DEFINITION
    {
        D2D1_INTERPOLATION_MODE_DEFINITION_NEAREST_NEIGHBOR = 0,
        D2D1_INTERPOLATION_MODE_DEFINITION_LINEAR = 1,
        D2D1_INTERPOLATION_MODE_DEFINITION_CUBIC = 2,
        D2D1_INTERPOLATION_MODE_DEFINITION_MULTI_SAMPLE_LINEAR = 3,
        D2D1_INTERPOLATION_MODE_DEFINITION_ANISOTROPIC = 4,
        D2D1_INTERPOLATION_MODE_DEFINITION_HIGH_QUALITY_CUBIC = 5,
        D2D1_INTERPOLATION_MODE_DEFINITION_FANT = 6,
        D2D1_INTERPOLATION_MODE_DEFINITION_MIPMAP_LINEAR = 7
    }

    internal enum D2D1_INTERPOLATION_MODE
    {
        D2D1_INTERPOLATION_MODE_NEAREST_NEIGHBOR = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_NEAREST_NEIGHBOR,
        D2D1_INTERPOLATION_MODE_LINEAR = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_LINEAR,
        D2D1_INTERPOLATION_MODE_CUBIC = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_CUBIC,
        D2D1_INTERPOLATION_MODE_MULTI_SAMPLE_LINEAR = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_MULTI_SAMPLE_LINEAR,
        D2D1_INTERPOLATION_MODE_ANISOTROPIC = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_ANISOTROPIC,
        D2D1_INTERPOLATION_MODE_HIGH_QUALITY_CUBIC = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_HIGH_QUALITY_CUBIC,
        D2D1_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_BITMAP_INTERPOLATION_MODE
    {
        //
        // Nearest Neighbor filtering. Also known as nearest pixel or nearest point
        // sampling.
        //
        D2D1_BITMAP_INTERPOLATION_MODE_NEAREST_NEIGHBOR = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_NEAREST_NEIGHBOR,
        //
        // Linear filtering.
        //
        D2D1_BITMAP_INTERPOLATION_MODE_LINEAR = D2D1_INTERPOLATION_MODE_DEFINITION.D2D1_INTERPOLATION_MODE_DEFINITION_LINEAR,
        D2D1_BITMAP_INTERPOLATION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_BITMAP_BRUSH_PROPERTIES1
    {
        public D2D1_EXTEND_MODE extendModeX;
        public D2D1_EXTEND_MODE extendModeY;
        public D2D1_INTERPOLATION_MODE interpolationMode;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_IMAGE_BRUSH_PROPERTIES
    {
        public D2D1_RECT_F sourceRectangle;
        public D2D1_EXTEND_MODE extendModeX;
        public D2D1_EXTEND_MODE extendModeY;
        public D2D1_INTERPOLATION_MODE interpolationMode;
    };

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_BRUSH_PROPERTIES
    {
        public float opacity;
        public D2D1_MATRIX_3X2_F transform;
    }

    internal enum DWRITE_MEASURING_MODE
    {
        //
        // Text is measured using glyph ideal metrics whose values are independent to the current display resolution.
        //
        DWRITE_MEASURING_MODE_NATURAL,
        //
        // Text is measured using glyph display compatible metrics whose values tuned for the current display resolution.
        //
        DWRITE_MEASURING_MODE_GDI_CLASSIC,
        //
        // Text is measured using the same glyph display metrics as text measured by GDI using a font
        // created with CLEARTYPE_NATURAL_QUALITY.
        //
        DWRITE_MEASURING_MODE_GDI_NATURAL
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct DWRITE_GLYPH_RUN
    {
        /// <summary>
        /// The physical font face to draw with.
        /// </summary>
        public IntPtr fontFace;
        /// <summary>
        /// Logical size of the font in DIPs, not points (equals 1/96 inch).
        /// </summary>
        public float fontEmSize;
        /// <summary>
        /// The number of glyphs.
        /// </summary>
        public uint glyphCount;
        /// <summary>
        /// The indices to render.
        /// </summary>
        public IntPtr glyphIndices;
        /// <summary>
        /// Glyph advance widths.
        /// </summary>
        public IntPtr glyphAdvances;
        /// <summary>
        /// Glyph offsets.
        /// </summary>
        public IntPtr glyphOffsets;
        /// <summary>
        /// If true, specifies that glyphs are rotated 90 degrees to the left and vertical metrics are used. Vertical writing is achieved by specifying isSideways = true and rotating the entire run 90 degrees to the right via a rotate transform.
        /// </summary>
        public bool isSideways;
        /// <summary>
        /// The implicit resolved bidi level of the run. Odd levels indicate right-to-left languages like Hebrew and Arabic, while even levels indicate left-to-right languages like English and Japanese (when written horizontally). For right-to-left languages, the text origin is on the right, and text should be drawn to the left.
        /// </summary>
        public uint bidiLevel;
    }

    internal enum D2D1_UNIT_MODE
    {
        D2D1_UNIT_MODE_DIPS = 0,
        D2D1_UNIT_MODE_PIXELS = 1,
        D2D1_UNIT_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_RENDERING_CONTROLS
    {
        /// <summary>
        /// The default buffer precision, used if the precision isn't otherwise specified.
        /// </summary>
        public D2D1_BUFFER_PRECISION bufferPrecision;

        /// <summary>
        /// The size of allocated tiles used to render imaging effects.
        /// </summary>
        public D2D1_SIZE_U tileSize;
    };

    internal enum D2D1_COMPOSITE_MODE
    {
        D2D1_COMPOSITE_MODE_SOURCE_OVER = 0,
        D2D1_COMPOSITE_MODE_DESTINATION_OVER = 1,
        D2D1_COMPOSITE_MODE_SOURCE_IN = 2,
        D2D1_COMPOSITE_MODE_DESTINATION_IN = 3,
        D2D1_COMPOSITE_MODE_SOURCE_OUT = 4,
        D2D1_COMPOSITE_MODE_DESTINATION_OUT = 5,
        D2D1_COMPOSITE_MODE_SOURCE_ATOP = 6,
        D2D1_COMPOSITE_MODE_DESTINATION_ATOP = 7,
        D2D1_COMPOSITE_MODE_XOR = 8,
        D2D1_COMPOSITE_MODE_PLUS = 9,
        D2D1_COMPOSITE_MODE_SOURCE_COPY = 10,
        D2D1_COMPOSITE_MODE_BOUNDED_SOURCE_COPY = 11,
        D2D1_COMPOSITE_MODE_MASK_INVERT = 12,
        D2D1_COMPOSITE_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_LAYER_PARAMETERS1
    {
        public D2D1_RECT_F contentBounds;
        public IntPtr geometricMask;
        public D2D1_ANTIALIAS_MODE maskAntialiasMode;
        public D2D1_MATRIX_3X2_F maskTransform;
        public float opacity;
        public IntPtr opacityBrush;
        public D2D1_LAYER_OPTIONS1 layerOptions;
    };

    internal enum D2D1_LAYER_OPTIONS1
    {
        D2D1_LAYER_OPTIONS1_NONE = 0,
        D2D1_LAYER_OPTIONS1_INITIALIZE_FROM_BACKGROUND = 1,
        D2D1_LAYER_OPTIONS1_IGNORE_ALPHA = 2,
        D2D1_LAYER_OPTIONS1_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_EFFECT_INPUT_DESCRIPTION
    {
        /// <summary>
        /// The effect whose input connection is being specified.
        /// </summary>
        public ID2D1Effect effect;

        /// <summary>
        /// The index of the input connection into the specified effect.
        /// </summary>
        uint inputIndex;

        /// <summary>
        /// The rectangle which would be available on the specified input connection during
        /// render operations.
        /// </summary>
        D2D1_RECT_F inputRectangle;
    };

    internal enum D2D1_BITMAP_OPTIONS
    {
        /// <summary>
        /// The bitmap is created with default properties.
        /// </summary>
        D2D1_BITMAP_OPTIONS_NONE = 0x00000000,

        /// <summary>
        /// The bitmap can be specified as a target in ID2D1DeviceContext::SetTarget
        /// </summary>
        D2D1_BITMAP_OPTIONS_TARGET = 0x00000001,

        /// <summary>
        /// The bitmap cannot be used as an input to DrawBitmap, DrawImage, in a bitmap
        /// brush or as an input to an effect.
        /// </summary>
        D2D1_BITMAP_OPTIONS_CANNOT_DRAW = 0x00000002,

        /// <summary>
        /// The bitmap can be read from the CPU.
        /// </summary>
        D2D1_BITMAP_OPTIONS_CPU_READ = 0x00000004,

        /// <summary>
        /// The bitmap works with the ID2D1GdiInteropRenderTarget::GetDC API.
        /// </summary>
        D2D1_BITMAP_OPTIONS_GDI_COMPATIBLE = 0x00000008,
        D2D1_BITMAP_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_BITMAP_PROPERTIES1
    {
        public D2D1_PIXEL_FORMAT pixelFormat;
        public float dpiX;
        public float dpiY;
        public D2D1_BITMAP_OPTIONS bitmapOptions;

        public IntPtr colorContext;
    };

    [ComImport]
    [Guid("65019f75-8da2-497c-b32c-dfa34e48ede6")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Image : ID2D1Resource
    {
        #region <ID2D1Resource>
        [PreserveSig]
        new void GetFactory(out ID2D1Factory factory);
        #endregion
    }

    [ComImport]
    [Guid("a2296057-ea42-4099-983b-539fb6505426")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Bitmap : ID2D1Image
    {
        #region <ID2D1Image>
        #region <ID2D1Resource>
        [PreserveSig]
        new void GetFactory(out ID2D1Factory factory);
        #endregion
        #endregion

        [PreserveSig]
        void GetSize(out D2D1_SIZE_F size);
        [PreserveSig]
        void GetPixelSize(out D2D1_SIZE_U size);
        [PreserveSig]
        void GetPixelFormat(out D2D1_PIXEL_FORMAT format);
        [PreserveSig]
        void GetDpi(out float dpiX, out float dpiY);
        [PreserveSig]
        void CopyFromBitmap(ref D2D1_POINT_2U destPoint, ID2D1Bitmap bitmap, D2D1_RECT_U srcRect);
        [PreserveSig]
        void CopyFromRenderTarget(ref D2D1_POINT_2U destPoint, ID2D1RenderTarget renderTarget, D2D1_RECT_U srcRect);
        [PreserveSig]
        void CopyFromMemory(ref D2D1_RECT_U dstRect, IntPtr srcData, uint pitch);
    }

    [ComImport]
    [Guid("a898a84c-3873-4588-b08b-ebbf978df041")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ID2D1Bitmap1 : ID2D1Bitmap
    {
        #region <ID2D1Bitmap>
        #region <ID2D1Image>
        #region <ID2D1Resource>
        new void GetFactory(out ID2D1Factory factory);
        #endregion
        #endregion
        [PreserveSig]
        new void GetSize(out D2D1_SIZE_F size);
        [PreserveSig]
        new void GetPixelSize(out D2D1_SIZE_U size);
        [PreserveSig]
        new void GetPixelFormat(out D2D1_PIXEL_FORMAT format);
        [PreserveSig]
        new void GetDpi(out float dpiX, out float dpiY);
        [PreserveSig]
        new void CopyFromBitmap(ref D2D1_POINT_2U destPoint, ID2D1Bitmap bitmap, D2D1_RECT_U srcRect);
        [PreserveSig]
        new void CopyFromRenderTarget(ref D2D1_POINT_2U destPoint, ID2D1RenderTarget renderTarget, D2D1_RECT_U srcRect);
        [PreserveSig]
        new void CopyFromMemory(ref D2D1_RECT_U dstRect, IntPtr srcData, uint pitch);
        #endregion

        [PreserveSig]
        void GetColorContext(out IntPtr colorContext);
        [PreserveSig]
        void GetOptions(out D2D1_BITMAP_OPTIONS options);
        [PreserveSig]
        HRESULT GetSurface(out IDXGISurface dxgiSurface);
        [PreserveSig]
        HRESULT Map(D2D1_MAP_OPTIONS options, out D2D1_MAPPED_RECT mappedRect);
        [PreserveSig]
        HRESULT Unmap();
    }

    internal enum D2D1_MAP_OPTIONS
    {
        /// <summary>
        /// The mapped pointer has undefined behavior.
        /// </summary>
        D2D1_MAP_OPTIONS_NONE = 0,

        /// <summary>
        /// The mapped pointer can be read from.
        /// </summary>
        D2D1_MAP_OPTIONS_READ = 1,

        /// <summary>
        /// The mapped pointer can be written to.
        /// </summary>
        D2D1_MAP_OPTIONS_WRITE = 2,

        /// <summary>
        /// The previous contents of the bitmap are discarded when it is mapped.
        /// </summary>
        D2D1_MAP_OPTIONS_DISCARD = 4,
        D2D1_MAP_OPTIONS_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct D2D1_MAPPED_RECT
    {
        public uint pitch;
        public IntPtr bits;
    };

    internal enum D2D1_EDGEDETECTION_PROP
    {

        /// <summary>
        /// Property Name: "Strength"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_EDGEDETECTION_PROP_STRENGTH = 0,

        /// <summary>
        /// Property Name: "BlurRadius"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_EDGEDETECTION_PROP_BLUR_RADIUS = 1,

        /// <summary>
        /// Property Name: "Mode"
        /// Property Type: D2D1_EDGEDETECTION_MODE
        /// </summary>
        D2D1_EDGEDETECTION_PROP_MODE = 2,

        /// <summary>
        /// Property Name: "OverlayEdges"
        /// Property Type: BOOL
        /// </summary>
        D2D1_EDGEDETECTION_PROP_OVERLAY_EDGES = 3,

        /// <summary>
        /// Property Name: "AlphaMode"
        /// Property Type: D2D1_ALPHA_MODE
        /// </summary>
        D2D1_EDGEDETECTION_PROP_ALPHA_MODE = 4,
        D2D1_EDGEDETECTION_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_EDGEDETECTION_MODE
    {
        D2D1_EDGEDETECTION_MODE_SOBEL = 0,
        D2D1_EDGEDETECTION_MODE_PREWITT = 1,
        D2D1_EDGEDETECTION_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    /// <summary>
    /// The enumeration of the Emboss effect's top level properties.
    /// Effect description: Applies an embossing effect to an image.
    /// </summary>
    internal enum D2D1_EMBOSS_PROP
    {
        /// <summary>
        /// Property Name: "Height"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_EMBOSS_PROP_HEIGHT = 0,

        /// <summary>
        /// Property Name: "Direction"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_EMBOSS_PROP_DIRECTION = 1,
        D2D1_EMBOSS_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_GAUSSIANBLUR_PROP
    {
        /// <summary>
        /// Property Name: "StandardDeviation"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_GAUSSIANBLUR_PROP_STANDARD_DEVIATION = 0,

        /// <summary>
        /// Property Name: "Optimization"
        /// Property Type: D2D1_GAUSSIANBLUR_OPTIMIZATION
        /// </summary>
        D2D1_GAUSSIANBLUR_PROP_OPTIMIZATION = 1,

        /// <summary>
        /// Property Name: "BorderMode"
        /// Property Type: D2D1_BORDER_MODE
        /// </summary>
        D2D1_GAUSSIANBLUR_PROP_BORDER_MODE = 2,
        D2D1_GAUSSIANBLUR_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_SHARPEN_PROP
    {
        /// <summary>
        /// Property Name: "Sharpness"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SHARPEN_PROP_SHARPNESS = 0,

        /// <summary>
        /// Property Name: "Threshold"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_SHARPEN_PROP_THRESHOLD = 1,
        D2D1_SHARPEN_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_GAUSSIANBLUR_OPTIMIZATION
    {
        D2D1_GAUSSIANBLUR_OPTIMIZATION_SPEED = 0,
        D2D1_GAUSSIANBLUR_OPTIMIZATION_BALANCED = 1,
        D2D1_GAUSSIANBLUR_OPTIMIZATION_QUALITY = 2,
        D2D1_GAUSSIANBLUR_OPTIMIZATION_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_COLORMATRIX_PROP
    {
        /// <summary>
        /// Property Name: "ColorMatrix"
        /// Property Type: D2D1_MATRIX_5X4_F
        /// </summary>
        D2D1_COLORMATRIX_PROP_COLOR_MATRIX = 0,

        /// <summary>
        /// Property Name: "AlphaMode"
        /// Property Type: D2D1_COLORMATRIX_ALPHA_MODE
        /// </summary>
        D2D1_COLORMATRIX_PROP_ALPHA_MODE = 1,

        /// <summary>
        /// Property Name: "ClampOutput"
        /// Property Type: BOOL
        /// </summary>
        D2D1_COLORMATRIX_PROP_CLAMP_OUTPUT = 2,
        D2D1_COLORMATRIX_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_COLORMATRIX_ALPHA_MODE
    {
        D2D1_COLORMATRIX_ALPHA_MODE_PREMULTIPLIED = 1,
        D2D1_COLORMATRIX_ALPHA_MODE_STRAIGHT = 2,
        D2D1_COLORMATRIX_ALPHA_MODE_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_BRIGHTNESS_PROP
    {
        /// <summary>
        /// Property Name: "WhitePoint"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_BRIGHTNESS_PROP_WHITE_POINT = 0,

        /// <summary>
        /// Property Name: "BlackPoint"
        /// Property Type: D2D1_VECTOR_2F
        /// </summary>
        D2D1_BRIGHTNESS_PROP_BLACK_POINT = 1,
        D2D1_BRIGHTNESS_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }

    internal enum D2D1_VIGNETTE_PROP
    {
        /// <summary>
        /// Property Name: "Color"
        /// Property Type: D2D1_VECTOR_4F
        /// </summary>
        D2D1_VIGNETTE_PROP_COLOR = 0,

        /// <summary>
        /// Property Name: "TransitionSize"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_VIGNETTE_PROP_TRANSITION_SIZE = 1,

        /// <summary>
        /// Property Name: "Strength"
        /// Property Type: FLOAT
        /// </summary>
        D2D1_VIGNETTE_PROP_STRENGTH = 2,
        D2D1_VIGNETTE_PROP_FORCE_DWORD = unchecked((int)0xffffffff)
    }


    internal class ColorF : D2D1_COLOR_F
    {
        public enum Enum
        {
            AliceBlue = 0xF0F8FF,
            AntiqueWhite = 0xFAEBD7,
            Aqua = 0x00FFFF,
            Aquamarine = 0x7FFFD4,
            Azure = 0xF0FFFF,
            Beige = 0xF5F5DC,
            Bisque = 0xFFE4C4,
            Black = 0x000000,
            BlanchedAlmond = 0xFFEBCD,
            Blue = 0x0000FF,
            BlueViolet = 0x8A2BE2,
            Brown = 0xA52A2A,
            BurlyWood = 0xDEB887,
            CadetBlue = 0x5F9EA0,
            Chartreuse = 0x7FFF00,
            Chocolate = 0xD2691E,
            Coral = 0xFF7F50,
            CornflowerBlue = 0x6495ED,
            Cornsilk = 0xFFF8DC,
            Crimson = 0xDC143C,
            Cyan = 0x00FFFF,
            DarkBlue = 0x00008B,
            DarkCyan = 0x008B8B,
            DarkGoldenrod = 0xB8860B,
            DarkGray = 0xA9A9A9,
            DarkGreen = 0x006400,
            DarkKhaki = 0xBDB76B,
            DarkMagenta = 0x8B008B,
            DarkOliveGreen = 0x556B2F,
            DarkOrange = 0xFF8C00,
            DarkOrchid = 0x9932CC,
            DarkRed = 0x8B0000,
            DarkSalmon = 0xE9967A,
            DarkSeaGreen = 0x8FBC8F,
            DarkSlateBlue = 0x483D8B,
            DarkSlateGray = 0x2F4F4F,
            DarkTurquoise = 0x00CED1,
            DarkViolet = 0x9400D3,
            DeepPink = 0xFF1493,
            DeepSkyBlue = 0x00BFFF,
            DimGray = 0x696969,
            DodgerBlue = 0x1E90FF,
            Firebrick = 0xB22222,
            FloralWhite = 0xFFFAF0,
            ForestGreen = 0x228B22,
            Fuchsia = 0xFF00FF,
            Gainsboro = 0xDCDCDC,
            GhostWhite = 0xF8F8FF,
            Gold = 0xFFD700,
            Goldenrod = 0xDAA520,
            Gray = 0x808080,
            Green = 0x008000,
            GreenYellow = 0xADFF2F,
            Honeydew = 0xF0FFF0,
            HotPink = 0xFF69B4,
            IndianRed = 0xCD5C5C,
            Indigo = 0x4B0082,
            Ivory = 0xFFFFF0,
            Khaki = 0xF0E68C,
            Lavender = 0xE6E6FA,
            LavenderBlush = 0xFFF0F5,
            LawnGreen = 0x7CFC00,
            LemonChiffon = 0xFFFACD,
            LightBlue = 0xADD8E6,
            LightCoral = 0xF08080,
            LightCyan = 0xE0FFFF,
            LightGoldenrodYellow = 0xFAFAD2,
            LightGreen = 0x90EE90,
            LightGray = 0xD3D3D3,
            LightPink = 0xFFB6C1,
            LightSalmon = 0xFFA07A,
            LightSeaGreen = 0x20B2AA,
            LightSkyBlue = 0x87CEFA,
            LightSlateGray = 0x778899,
            LightSteelBlue = 0xB0C4DE,
            LightYellow = 0xFFFFE0,
            Lime = 0x00FF00,
            LimeGreen = 0x32CD32,
            Linen = 0xFAF0E6,
            Magenta = 0xFF00FF,
            Maroon = 0x800000,
            MediumAquamarine = 0x66CDAA,
            MediumBlue = 0x0000CD,
            MediumOrchid = 0xBA55D3,
            MediumPurple = 0x9370DB,
            MediumSeaGreen = 0x3CB371,
            MediumSlateBlue = 0x7B68EE,
            MediumSpringGreen = 0x00FA9A,
            MediumTurquoise = 0x48D1CC,
            MediumVioletRed = 0xC71585,
            MidnightBlue = 0x191970,
            MintCream = 0xF5FFFA,
            MistyRose = 0xFFE4E1,
            Moccasin = 0xFFE4B5,
            NavajoWhite = 0xFFDEAD,
            Navy = 0x000080,
            OldLace = 0xFDF5E6,
            Olive = 0x808000,
            OliveDrab = 0x6B8E23,
            Orange = 0xFFA500,
            OrangeRed = 0xFF4500,
            Orchid = 0xDA70D6,
            PaleGoldenrod = 0xEEE8AA,
            PaleGreen = 0x98FB98,
            PaleTurquoise = 0xAFEEEE,
            PaleVioletRed = 0xDB7093,
            PapayaWhip = 0xFFEFD5,
            PeachPuff = 0xFFDAB9,
            Peru = 0xCD853F,
            Pink = 0xFFC0CB,
            Plum = 0xDDA0DD,
            PowderBlue = 0xB0E0E6,
            Purple = 0x800080,
            Red = 0xFF0000,
            RosyBrown = 0xBC8F8F,
            RoyalBlue = 0x4169E1,
            SaddleBrown = 0x8B4513,
            Salmon = 0xFA8072,
            SandyBrown = 0xF4A460,
            SeaGreen = 0x2E8B57,
            SeaShell = 0xFFF5EE,
            Sienna = 0xA0522D,
            Silver = 0xC0C0C0,
            SkyBlue = 0x87CEEB,
            SlateBlue = 0x6A5ACD,
            SlateGray = 0x708090,
            Snow = 0xFFFAFA,
            SpringGreen = 0x00FF7F,
            SteelBlue = 0x4682B4,
            Tan = 0xD2B48C,
            Teal = 0x008080,
            Thistle = 0xD8BFD8,
            Tomato = 0xFF6347,
            Turquoise = 0x40E0D0,
            Violet = 0xEE82EE,
            Wheat = 0xF5DEB3,
            White = 0xFFFFFF,
            WhiteSmoke = 0xF5F5F5,
            Yellow = 0xFFFF00,
            YellowGreen = 0x9ACD32,
        };

        //
        // Construct a color, note that the alpha value from the "rgb" component
        // is never used.
        // 

        public ColorF(uint rgb, float a = 1.0f)
        {
            Init(rgb, a);
        }

        public ColorF(Enum knownColor, float a = 1.0f)
        {
            Init((uint)knownColor, a);
        }
        public ColorF(float red, float green, float blue, float alpha = 1.0f)
        {
            r = red;
            g = green;
            b = blue;
            a = alpha;
        }
        void Init(uint rgb, float alpha)
        {
            r = ((rgb & sc_redMask) >> (int)sc_redShift) / 255.0f;
            g = ((rgb & sc_greenMask) >> (int)sc_greenShift) / 255.0f;
            b = ((rgb & sc_blueMask) >> (int)sc_blueShift) / 255.0f;
            a = alpha;
        }
        const uint sc_redShift = 16;
        const uint sc_greenShift = 8;
        const uint sc_blueShift = 0;

        const uint sc_redMask = 0xff << (int)sc_redShift;
        const uint sc_greenMask = 0xff << (int)sc_greenShift;
        const uint sc_blueMask = 0xff << (int)sc_blueShift;
    };

    internal class Matrix3x2F : D2D1_MATRIX_3X2_F
    {
        public Matrix3x2F(float m11, float m12, float m21, float m22, float m31, float m32)
        {
            _11 = m11;
            _12 = m12;
            _21 = m21;
            _22 = m22;
            _31 = m31;
            _32 = m32;
        }

        public Matrix3x2F()
        {
        }

        public static Matrix3x2F Identity()
        {
            return new Matrix3x2F(1f, 0f, 0f, 1f, 0f, 0f);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct Matrix3x2
        {
            /// <summary>
            /// Gets the identity matrix.
            /// </summary>
            /// <value>The identity matrix.</value>
            public readonly static Matrix3x2 Identity = new Matrix3x2(1, 0, 0, 1, 0, 0);

            /// <summary>
            /// Element (1,1)
            /// </summary>
            public float M11;

            /// <summary>
            /// Element (1,2)
            /// </summary>
            public float M12;

            /// <summary>
            /// Element (2,1)
            /// </summary>
            public float M21;

            /// <summary>
            /// Element (2,2)
            /// </summary>
            public float M22;

            /// <summary>
            /// Element (3,1)
            /// </summary>
            public float M31;

            /// <summary>
            /// Element (3,2)
            /// </summary>
            public float M32;

            /// <summary>
            /// Initializes a new instance of the <see cref="Matrix3x2"/> struct.
            /// </summary>
            /// <param name="value">The value that will be assigned to all components.</param>
            public Matrix3x2(float value)
            {
                M11 = M12 =
                M21 = M22 =
                M31 = M32 = value;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Matrix3x2"/> struct.
            /// </summary>
            /// <param name="M11">The value to assign at row 1 column 1 of the matrix.</param>
            /// <param name="M12">The value to assign at row 1 column 2 of the matrix.</param>
            /// <param name="M21">The value to assign at row 2 column 1 of the matrix.</param>
            /// <param name="M22">The value to assign at row 2 column 2 of the matrix.</param>
            /// <param name="M31">The value to assign at row 3 column 1 of the matrix.</param>
            /// <param name="M32">The value to assign at row 3 column 2 of the matrix.</param>
            public Matrix3x2(float M11, float M12, float M21, float M22, float M31, float M32)
            {
                this.M11 = M11; this.M12 = M12;
                this.M21 = M21; this.M22 = M22;
                this.M31 = M31; this.M32 = M32;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Matrix3x2"/> struct.
            /// </summary>
            /// <param name="values">The values to assign to the components of the matrix. This must be an array with six elements.</param>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than six elements.</exception>
            public Matrix3x2(float[] values)
            {
                if (values == null)
                    throw new ArgumentNullException("values");
                if (values.Length != 6)
                    throw new ArgumentOutOfRangeException("values", "There must be six input values for Matrix3x2.");

                M11 = values[0];
                M12 = values[1];

                M21 = values[2];
                M22 = values[3];

                M31 = values[4];
                M32 = values[5];
            }
        }

        //public static void MakeRotateMatrix(float angle, D2D1_POINT_2F center, out Matrix3x2 matrix)
        //{
        //    unsafe
        //    {
        //        IntPtr pRotation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(D2D1_MATRIX_3X2_F)));
        //        D2D1_MATRIX_3X2_F rotation = new D2D1_MATRIX_3X2_F();
        //        D2DTools.D2D1MakeRotateMatrix_(angle, center,(void*) pRotation);
        //        rotation = (D2D1_MATRIX_3X2_F)Marshal.PtrToStructure(pRotation, typeof(D2D1_MATRIX_3X2_F));

        //        matrix = new Matrix3x2();
        //        fixed (void* matrix_ = &matrix)
        //            D2DTools.D2D1MakeRotateMatrix_(angle, center, matrix_);               
        //    }
        //}

        public static Matrix3x2 MakeRotateMatrix(float angle, D2D1_POINT_2F center = new D2D1_POINT_2F())
        {
            unsafe
            {
                IntPtr pRotation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(D2D1_MATRIX_3X2_F)));
                D2D1_MATRIX_3X2_F rotation = new D2D1_MATRIX_3X2_F();
                DirectXInternalTools.D2D1MakeRotateMatrix_(angle, center, (void*)pRotation);
                rotation = (D2D1_MATRIX_3X2_F)Marshal.PtrToStructure(pRotation, typeof(D2D1_MATRIX_3X2_F));

                Matrix3x2 matrix = new Matrix3x2();
                void* matrix_ = &matrix;
                DirectXInternalTools.D2D1MakeRotateMatrix_(angle, center, matrix_);
                return matrix;
            }
        }

        public static Matrix3x2F Rotation(float angle, D2D1_POINT_2F center = new D2D1_POINT_2F())
        {
            D2D1_MATRIX_3X2_F rotation = new D2D1_MATRIX_3X2_F();
            IntPtr pRotation = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(D2D1_MATRIX_3X2_F)));

            //Marshal.StructureToPtr(rotation, pRotation, false);

            unsafe
            {
                //D2D1_MATRIX_3X2_F_BIS rot2 = new D2D1_MATRIX_3X2_F_BIS();
                DirectXInternalTools.D2D1MakeRotateMatrix_(angle, center, (void*)pRotation);
            }

            //IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(D2D1_MATRIX_3X2_F_BIS)));
            //D2DTools.D2D1MakeRotateMatrix(angle, center, out pRotation);
            // D2DTools.D2D1MakeRotateMatrix(angle, center, out p);

            //rot2 = (D2D1_MATRIX_3X2_F_BIS)Marshal.PtrToStructure(p, typeof(D2D1_MATRIX_3X2_F_BIS));

            //var ItemIDList = (D2D1_MATRIX_3X2_F)Marshal.PtrToStructure(pRotation, typeof(D2D1_MATRIX_3X2_F));
            rotation = (D2D1_MATRIX_3X2_F)Marshal.PtrToStructure(pRotation, typeof(D2D1_MATRIX_3X2_F));
            Matrix3x2F m = new Matrix3x2F(rotation._11, rotation._12, rotation._21, rotation._22, rotation._31, rotation._32);

            //D2DTools.D2D1MakeRotateMatrix(angle, center, out rotation);    

            return m;
        }

        public static Matrix3x2F Skew(float angleX, float angleY, D2D1_POINT_2F center = default)
        {
            float tanX = (float)Math.Tan(angleX * (float)Math.PI / 180f);
            float tanY = (float)Math.Tan(angleY * (float)Math.PI / 180f);

            return new Matrix3x2F(
                1f, tanY, tanX, 1f,
                -tanX * center.y, -tanY * center.x
            );
        }

        public static Matrix3x2F Scale(D2D1_SIZE_F size, D2D1_POINT_2F center = new D2D1_POINT_2F())
        {
            Matrix3x2F scale = new Matrix3x2F();
            scale._11 = size.width; scale._12 = 0.0f;
            scale._21 = 0.0f; scale._22 = size.height;
            scale._31 = center.x - size.width * center.x;
            scale._32 = center.y - size.height * center.y;
            return scale;
        }

        public static Matrix3x2F Scale(float x, float y, D2D1_POINT_2F center = new D2D1_POINT_2F())
        {
            return Scale(new D2D1_SIZE_F(x, y), center);
        }

        public static Matrix3x2F Translation(D2D1_SIZE_F size)
        {
            Matrix3x2F translation = new Matrix3x2F();
            translation._11 = 1.0f; translation._12 = 0.0f;
            translation._21 = 0.0f; translation._22 = 1.0f;
            translation._31 = size.width; translation._32 = size.height;
            return translation;
        }

        public static Matrix3x2F Translation(float x, float y)
        {
            return Translation(new D2D1_SIZE_F(x, y));
        }

        public float Determinant()
        {
            return _11 * _22 - _12 * _21;
        }

        public bool IsInvertible()
        {
            return Determinant() != 0;
        }

        public bool Invert()
        {
            float det = Determinant();
            if (det == 0)
            {
                return false;
            }

            float invDet = 1f / det;
            float m11 = _22 * invDet;
            float m12 = -_12 * invDet;
            float m21 = -_21 * invDet;
            float m22 = _11 * invDet;
            float m31 = (_21 * _32 - _22 * _31) * invDet;
            float m32 = (_12 * _31 - _11 * _32) * invDet;

            _11 = m11;
            _12 = m12;
            _21 = m21;
            _22 = m22;
            _31 = m31;
            _32 = m32;

            return true;
        }

        public bool IsIdentity()
        {
            return _11 == 1f && _12 == 0f && _21 == 0f && _22 == 1f && _31 == 0f && _32 == 0f;
        }

        public void SetProduct(Matrix3x2F a, Matrix3x2F b)
        {
            _11 = a._11 * b._11 + a._12 * b._21;
            _12 = a._11 * b._12 + a._12 * b._22;
            _21 = a._21 * b._11 + a._22 * b._21;
            _22 = a._21 * b._12 + a._22 * b._22;
            _31 = a._31 * b._11 + a._32 * b._21 + b._31;
            _32 = a._31 * b._12 + a._32 * b._22 + b._32;
        }

        public static Matrix3x2F operator *(Matrix3x2F b, Matrix3x2F c)
        {
            Matrix3x2F result = new Matrix3x2F();
            result.SetProduct(b, c);
            return result;
        }

        public D2D1_POINT_2F TransformPoint(D2D1_POINT_2F point)
        {
            return new D2D1_POINT_2F(
                point.x * _11 + point.y * _21 + _31,
                point.x * _12 + point.y * _22 + _32
            );
        }
    }

    public sealed class DirectXInternalTools
    {
        public static Guid CLSID_D2D1Factory = new Guid("06152247-6f50-465a-9245-118bfd3b6007");

        public static Guid CLSID_D2D12DAffineTransform = new Guid("{6AA97485-6354-4CFC-908C-E4A74F62C96C}");
        public static Guid CLSID_D2D13DPerspectiveTransform = new Guid("{C2844D0B-3D86-46E7-85BA-526C9240F3FB}");
        public static Guid CLSID_D2D13DTransform = new Guid("{E8467B04-EC61-4B8A-B5DE-D4D73DEBEA5A}");
        public static Guid CLSID_D2D1ArithmeticComposite = new Guid("{FC151437-049A-4784-A24A-F1C4DAF20987}");
        public static Guid CLSID_D2D1Atlas = new Guid("{913E2BE4-FDCF-4FE2-A5F0-2454F14FF408}");
        public static Guid CLSID_D2D1BitmapSource = new Guid("{5FB6C24D-C6DD-4231-9404-50F4D5C3252D}");
        public static Guid CLSID_D2D1Blend = new Guid("{81C5B77B-13F8-4CDD-AD20-C890547AC65D}");
        public static Guid CLSID_D2D1Border = new Guid("{2A2D49C0-4ACF-43C7-8C6A-7C4A27874D27}");
        public static Guid CLSID_D2D1Brightness = new Guid("{8CEA8D1E-77B0-4986-B3B9-2F0C0EAE7887}");
        public static Guid CLSID_D2D1ColorManagement = new Guid("{1A28524C-FDD6-4AA4-AE8F-837EB8267B37}");
        public static Guid CLSID_D2D1ColorMatrix = new Guid("{921F03D6-641C-47DF-852D-B4BB6153AE11}");
        public static Guid CLSID_D2D1Composite = new Guid("{48FC9F51-F6AC-48F1-8B58-3B28AC46F76D}");
        public static Guid CLSID_D2D1ConvolveMatrix = new Guid("{407F8C08-5533-4331-A341-23CC3877843E}");
        public static Guid CLSID_D2D1Crop = new Guid("{E23F7110-0E9A-4324-AF47-6A2C0C46F35B}");
        public static Guid CLSID_D2D1DirectionalBlur = new Guid("{174319A6-58E9-49B2-BB63-CAF2C811A3DB}");
        public static Guid CLSID_D2D1DiscreteTransfer = new Guid("{90866FCD-488E-454B-AF06-E5041B66C36C}");
        public static Guid CLSID_D2D1DisplacementMap = new Guid("{EDC48364-0417-4111-9450-43845FA9F890}");
        public static Guid CLSID_D2D1DistantDiffuse = new Guid("{3E7EFD62-A32D-46D4-A83C-5278889AC954}");
        public static Guid CLSID_D2D1DistantSpecular = new Guid("{428C1EE5-77B8-4450-8AB5-72219C21ABDA}");
        public static Guid CLSID_D2D1DpiCompensation = new Guid("{6C26C5C7-34E0-46FC-9CFD-E5823706E228}");
        public static Guid CLSID_D2D1Flood = new Guid("{61C23C20-AE69-4D8E-94CF-50078DF638F2}");
        public static Guid CLSID_D2D1GammaTransfer = new Guid("{409444C4-C419-41A0-B0C1-8CD0C0A18E42}");
        public static Guid CLSID_D2D1GaussianBlur = new Guid("{1FEB6D69-2FE6-4AC9-8C58-1D7F93E7A6A5}");
        public static Guid CLSID_D2D1Scale = new Guid("{9DAF9369-3846-4D0E-A44E-0C607934A5D7}");
        public static Guid CLSID_D2D1Histogram = new Guid("{881DB7D0-F7EE-4D4D-A6D2-4697ACC66EE8}");
        public static Guid CLSID_D2D1HueRotation = new Guid("{0F4458EC-4B32-491B-9E85-BD73F44D3EB6}");
        public static Guid CLSID_D2D1LinearTransfer = new Guid("{AD47C8FD-63EF-4ACC-9B51-67979C036C06}");
        public static Guid CLSID_D2D1LuminanceToAlpha = new Guid("{41251AB7-0BEB-46F8-9DA7-59E93FCCE5DE}");
        public static Guid CLSID_D2D1Morphology = new Guid("{EAE6C40D-626A-4C2D-BFCB-391001ABE202}");
        public static Guid CLSID_D2D1OpacityMetadata = new Guid("{6C53006A-4450-4199-AA5B-AD1656FECE5E}");
        public static Guid CLSID_D2D1PointDiffuse = new Guid("{B9E303C3-C08C-4F91-8B7B-38656BC48C20}");
        public static Guid CLSID_D2D1PointSpecular = new Guid("{09C3CA26-3AE2-4F09-9EBC-ED3865D53F22}");
        public static Guid CLSID_D2D1Premultiply = new Guid("{06EAB419-DEED-4018-80D2-3E1D471ADEB2}");
        public static Guid CLSID_D2D1Saturation = new Guid("{5CB2D9CF-327D-459F-A0CE-40C0B2086BF7}");
        public static Guid CLSID_D2D1Shadow = new Guid("{C67EA361-1863-4E69-89DB-695D3E9A5B6B}");
        public static Guid CLSID_D2D1SpotDiffuse = new Guid("{818A1105-7932-44F4-AA86-08AE7B2F2C93}");
        public static Guid CLSID_D2D1SpotSpecular = new Guid("{EDAE421E-7654-4A37-9DB8-71ACC1BEB3C1}");
        public static Guid CLSID_D2D1TableTransfer = new Guid("{5BF818C3-5E43-48CB-B631-868396D6A1D4}");
        public static Guid CLSID_D2D1Tile = new Guid("{B0784138-3B76-4BC5-B13B-0FA2AD02659F}");
        public static Guid CLSID_D2D1Turbulence = new Guid("{CF2BB6AE-889A-4AD7-BA29-A2FD732C9FC9}");
        public static Guid CLSID_D2D1UnPremultiply = new Guid("{FB9AC489-AD8D-41ED-9999-BB6347D110F7}");

        public static Guid CLSID_D2D1Contrast = new Guid("{B648A78A-0ED5-4F80-A94A-8E825ACA6B77}");
        public static Guid CLSID_D2D1RgbToHue = new Guid("{23F3E5EC-91E8-4D3D-AD0A-AFADC1004AA1}");
        public static Guid CLSID_D2D1HueToRgb = new Guid("{7B78A6BD-0141-4DEF-8A52-6356EE0CBDD5}");
        public static Guid CLSID_D2D1ChromaKey = new Guid("{74C01F5B-2A0D-408C-88E2-C7A3C7197742}");
        public static Guid CLSID_D2D1Emboss = new Guid("{B1C5EB2B-0348-43F0-8107-4957CACBA2AE}");
        public static Guid CLSID_D2D1Exposure = new Guid("{B56C8CFA-F634-41EE-BEE0-FFA617106004}");
        public static Guid CLSID_D2D1Grayscale = new Guid("{36DDE0EB-3725-42E0-836D-52FB20AEE644}");
        public static Guid CLSID_D2D1Invert = new Guid("{E0C3784D-CB39-4E84-B6FD-6B72F0810263}");
        public static Guid CLSID_D2D1Posterize = new Guid("{2188945E-33A3-4366-B7BC-086BD02D0884}");
        public static Guid CLSID_D2D1Sepia = new Guid("{3A1AF410-5F1D-4DBE-84DF-915DA79B7153}");
        public static Guid CLSID_D2D1Sharpen = new Guid("{C9B887CB-C5FF-4DC5-9779-273DCF417C7D}");
        public static Guid CLSID_D2D1Straighten = new Guid("{4DA47B12-79A3-4FB0-8237-BBC3B2A4DE08}");
        public static Guid CLSID_D2D1TemperatureTint = new Guid("{89176087-8AF9-4A08-AEB1-895F38DB1766}");
        public static Guid CLSID_D2D1Vignette = new Guid("{C00C40BE-5E67-4CA3-95B4-F4B02C115135}");
        public static Guid CLSID_D2D1EdgeDetection = new Guid("{EFF583CA-CB07-4AA9-AC5D-2CC44C76460F}");
        public static Guid CLSID_D2D1HighlightsShadows = new Guid("{CADC8384-323F-4C7E-A361-2E2B24DF6EE4}");
        public static Guid CLSID_D2D1LookupTable3D = new Guid("{349E0EDA-0088-4A79-9CA3-C7E300202020}");
        public static Guid CLSID_D2D1Opacity = new Guid("{811D79A4-DE28-4454-8094-C64685F8BD4C}");
        public static Guid CLSID_D2D1AlphaMask = new Guid("{C80ECFF0-3FD5-4F05-8328-C5D1724B4F0A}");
        public static Guid CLSID_D2D1CrossFade = new Guid("{12F575E8-4DB1-485F-9A84-03A07DD3829F}");
        public static Guid CLSID_D2D1Tint = new Guid("{36312B17-F7DD-4014-915D-FFCA768CF211}");
        public static Guid CLSID_D2D1WhiteLevelAdjustment = new Guid("{44A1CADB-6CDD-4818-8FF4-26C1CFE95BDB}");
        public static Guid CLSID_D2D1HdrToneMap = new Guid("{7B0B748D-4610-4486-A90C-999D9A2E2B11}");

        [DllImport("D2D1.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern HRESULT D2D1CreateFactory(D2D1_FACTORY_TYPE factoryType, ref Guid riid, ref D2D1_FACTORY_OPTIONS pFactoryOptions, out ID2D1Factory ppIFactory);

        // [Build] "Allow unsafe code"
        [DllImport("D2D1.dll", EntryPoint = "D2D1MakeRotateMatrix", CallingConvention = CallingConvention.StdCall)]
        internal unsafe static extern void D2D1MakeRotateMatrix_(float angle, D2D1_POINT_2F center, void* matrix);

        internal static void SafeRelease<T>(ref T comObject) where T : class
        {
            T t = comObject;
            comObject = default(T);
            if (null != t)
            {
                if (Marshal.IsComObject(t))
                    Marshal.ReleaseComObject(t);
            }            
        }

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint GetDpiForWindow(IntPtr hwnd);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        internal static void SetEffectFloatArray(ID2D1Effect pEffect, uint nEffect, float[] aFloatArray)
        {
            int nDataSize = aFloatArray.Length * Marshal.SizeOf(typeof(float));
            IntPtr pData = Marshal.AllocHGlobal(nDataSize);
            Marshal.Copy(aFloatArray, 0, pData, aFloatArray.Length);
            HRESULT hr = pEffect.SetValue(nEffect, D2D1_PROPERTY_TYPE.D2D1_PROPERTY_TYPE_UNKNOWN, pData, (uint)nDataSize);
            Marshal.FreeHGlobal(pData);
        }

        internal static void SetEffectFloat(ID2D1Effect pEffect, uint nEffect, float fValue)
        {
            float[] aFloatArray = { fValue };
            int nDataSize = aFloatArray.Length * Marshal.SizeOf(typeof(float));
            IntPtr pData = Marshal.AllocHGlobal(nDataSize);
            Marshal.Copy(aFloatArray, 0, pData, aFloatArray.Length);
            HRESULT hr = pEffect.SetValue(nEffect, D2D1_PROPERTY_TYPE.D2D1_PROPERTY_TYPE_UNKNOWN, pData, (uint)nDataSize);
            Marshal.FreeHGlobal(pData);
        }

        internal static void SetEffectInt(ID2D1Effect pEffect, uint nEffect, uint nValue)
        {
            IntPtr pData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(Int32)));
            Marshal.WriteInt32(pData, (int)nValue);
            HRESULT hr = pEffect.SetValue(nEffect, D2D1_PROPERTY_TYPE.D2D1_PROPERTY_TYPE_UNKNOWN, pData, (uint)Marshal.SizeOf(typeof(Int32)));
            Marshal.FreeHGlobal(pData);
        }

        internal static void SetEffectIntPtr(ID2D1Effect pEffect, uint nEffect, IntPtr pPointer)
        {
            IntPtr pData = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            Marshal.WriteIntPtr(pData, pPointer);
            HRESULT hr = pEffect.SetValue(nEffect, D2D1_PROPERTY_TYPE.D2D1_PROPERTY_TYPE_UNKNOWN, pData, (uint)Marshal.SizeOf(typeof(IntPtr)));
            Marshal.FreeHGlobal(pData);
        }
    }
}