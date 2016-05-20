namespace NativeObject
{
    
    public class MI_Serializer
    {
    }

    public class MI_Deserializer
    {
    }

    public enum MI_CallbackMode : uint
    {
        Report = 0,
        Inquire = 1,
	Ignore = 2,
    }

    public enum MI_ImpersonationType
    {
	Default = 0,
	None = 1,
	Identify = 2,
	Impersonate = 3,
	Delegate = 4,
    }
    
    public class MI_ProxyType
    {
	public static string None = "None";
	public static string WinHTTP = "WinHTTP";
	public static string Auto = "Auto";
	public static string IE = "IE";
    }

    public class MI_PacketEncoding
    {
	public static string Default = "default";
	public static string UTF8 = "UTF8";
	public static string UTF16 = "UTF16";
    }

    public class MI_Protocol
    {
	public static string WSMan = "WinRM";
    }

    public class MI_Transport
    {
	public static string HTTPS = "HTTPS";
	public static string HTTP = "HTTP";
    }

    class OperationCallbackProcessingContext
    {
	private bool inUserCode;
	private object managedOperationContext;
	
	internal OperationCallbackProcessingContext(object managedOperationContext)
	{
	    this.inUserCode = false;
	    this.managedOperationContext = managedOperationContext;
	}
	
	internal bool InUserCode 
	{
	    get
	    { 
		return this.inUserCode;
	    }
	    set
	    {
		this.inUserCode = value;
	    }
	}

	internal object ManagedOperationContext
	{
	    get
	    { 
		return this.managedOperationContext; 
	    }
	}
    }


    class InstanceMethods
    {
	internal static void ThrowIfMismatchedType(MI_Type type, object managedValue)
	{
	    // TODO: Implement this
	    /*
	      MI_Value throwAway;
	      memset(&throwAway, 0, sizeof(MI_Value));
	      IEnumerable<DangerousHandleAccessor^>^ dangerousHandleAccesorsFromConversion = nullptr;
	      try
	      {
	      dangerousHandleAccesorsFromConversion = ConvertToMiValue(type, managedValue, &throwAway);
	      }
	      finally
	      {
	      ReleaseMiValue(type, &throwAway, dangerousHandleAccesorsFromConversion);
	      }
	    */
	}
    }
    
}