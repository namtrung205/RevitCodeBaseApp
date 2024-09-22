using System ;
using System.Diagnostics ;
using System.Runtime.InteropServices ;
using System.Windows.Input ;

namespace Helpers.Utils ;

public class LowLevelKeyboardListener
{
    private const int WhKeyBoarDLl = 13;
    private const int WmKeyDown = 0x0100;
    private const int WmSysKeyDown = 0x0104;
 
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lowLevelKeyboardProc, IntPtr hMod, uint dwThreadId);
 
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);
 
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
 
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
 
    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
 
    public event EventHandler<KeyPressedArgs>? OnKeyPressed;
 
    private LowLevelKeyboardProc _lowLevelKeyboardProc;
    private IntPtr _hookId = IntPtr.Zero;
 
    public LowLevelKeyboardListener()
    {
        _lowLevelKeyboardProc = HookCallback;
    }
 
    public void HookKeyboard()
    {
        _hookId = SetHook(_lowLevelKeyboardProc);
    }
 
    public void UnHookKeyboard()
    {
        UnhookWindowsHookEx(_hookId);
    }
 
    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        var curProcess = Process.GetCurrentProcess() ;
        var curModule = curProcess.MainModule ;
        if ( curModule != null ) return SetWindowsHookEx( WhKeyBoarDLl, proc, GetModuleHandle( curModule.ModuleName ), 0 ) ;
        return new IntPtr() ;
    }
 
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WmKeyDown || wParam == (IntPtr)WmSysKeyDown)
        {
            int vkCode = Marshal.ReadInt32(lParam);
 
            if (OnKeyPressed != null) { OnKeyPressed(this, new KeyPressedArgs(KeyInterop.KeyFromVirtualKey(vkCode))); }
        }
 
        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }
}
 
public class KeyPressedArgs( Key key ) : EventArgs
{
    public Key KeyPressed { get; private set; } = key ;
}