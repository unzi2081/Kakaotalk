using System;
using System.Text;
using System.Runtime.InteropServices;

public static class WindowEnumerator
{
    // 윈도우 핸들을 열거하는 함수
    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    // 자식 윈도우들을 열거하는 함수
    [DllImport("user32.dll")]
    public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

    // 윈도우 제목을 가져오는 함수
    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int nMaxCount);

    // 윈도우 클래스 이름을 가져오는 함수
    [DllImport("user32.dll")]
    public static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

    // 윈도우 프로세스 ID를 가져오는 함수
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

    // EnumWindows 프로시저 델리게이트 정의
    public delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

    // 사용 예시: 윈도우 핸들을 열거하는 코드
    public static void EnumWindows(IntPtr hwnd, EnumWindowsProc lpEnumFunc)
    {
        EnumWindows(lpEnumFunc, IntPtr.Zero);
    }
}
