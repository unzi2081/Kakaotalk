using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat_room_list
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        // EnumWindows callback delegate
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // EnumChildWindows 함수 정의
        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

        // EnumChildWindows callback delegate
        public delegate bool EnumChildProc(IntPtr hwnd, IntPtr lParam);

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

        }

        private void ListChatWindows()
        {
            Process[] kakaoProcesses = Process.GetProcessesByName("KakaoTalk");

            if (kakaoProcesses.Length == 0)
            {
                listBox1.Items.Add("❌ KakaoTalk.exe 프로세스를 찾을 수 없습니다.");
                return;
            }

            HashSet<uint> kakaoPids = new HashSet<uint>();
            foreach (var process in kakaoProcesses)
            {
                kakaoPids.Add((uint)process.Id);
            }

            string[] excludeTitles = {
        "_eva_Win32Timer_Window",
        "GDI+ Window (KakaoTalk.exe)",
        "MSCTFIME UI",
        "Default IME"
    };

            List<string> chatRoomTitles = new List<string>();

            // 모든 윈도우를 열거하여 KakaoTalk 프로세스에 해당하는 창들을 찾아 목록에 추가
            EnumWindows((hWnd, lParam) =>
            {
                StringBuilder title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);

                if (title.Length > 0)
                {
                    GetWindowThreadProcessId(hWnd, out uint pid);
                    if (kakaoPids.Contains(pid))
                    {
                        string windowTitle = title.ToString();

                        // 제외 목록에 있는 창이면 패스
                        if (Array.Exists(excludeTitles, exclude => windowTitle.Equals(exclude, StringComparison.OrdinalIgnoreCase)))
                            return true;

                        IntPtr richEditHandle = IntPtr.Zero;

                        // 자식 윈도우들을 열거하여 RICHEDIT50W 컨트롤을 찾음
                        EnumChildWindows(hWnd, (childHwnd, lP) =>
                        {
                            StringBuilder className = new StringBuilder(256);
                            GetClassName(childHwnd, className, className.Capacity);

                            if (className.ToString() == "RICHEDIT50W")
                            {
                                richEditHandle = childHwnd;
                                return false; // 찾았으므로 중지
                            }
                            return true;
                        }, IntPtr.Zero);

                        // RICHEDIT50W 핸들이 있는 창만 추가
                        if (richEditHandle != IntPtr.Zero)
                        {
                            chatRoomTitles.Add(windowTitle); // 채팅방 제목만 추가
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);

            // 채팅방 리스트를 listBox1에 추가
            if (chatRoomTitles.Count > 0)
            {
                foreach (var title in chatRoomTitles)
                {
                    listBox1.Items.Add(title); // 채팅방 제목만 리스트에 추가
                }
            }
            else
            {
                listBox1.Items.Add("❌ 채팅방을 찾을 수 없습니다.");
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ListChatWindows();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string selectedRoom = listBox1.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedRoom))
            {
                MessageBox.Show("채팅방을 선택하세요.");
                return;
            }

            // KakaoTalk 프로세스 가져오기
            Process[] kakaoProcesses = Process.GetProcessesByName("KakaoTalk");

            if (kakaoProcesses.Length == 0)
            {
                MessageBox.Show("❌ KakaoTalk.exe 프로세스를 찾을 수 없습니다.");
                return;
            }

            HashSet<uint> kakaoPids = new HashSet<uint>();
            foreach (var process in kakaoProcesses)
            {
                kakaoPids.Add((uint)process.Id);
            }

            IntPtr selectedHandle = IntPtr.Zero;

            // 모든 윈도우를 열거하여 KakaoTalk 프로세스에 해당하는 창들을 찾아 선택한 채팅방을 찾음
            WindowEnumerator.EnumWindows((hWnd, lParam) =>
            {
                StringBuilder title = new StringBuilder(256);
                WindowEnumerator.GetWindowText(hWnd, title, title.Capacity);

                if (title.Length > 0)
                {
                    WindowEnumerator.GetWindowThreadProcessId(hWnd, out uint pid);
                    if (kakaoPids.Contains(pid))
                    {
                        string windowTitle = title.ToString();

                        if (windowTitle == selectedRoom)
                        {
                            IntPtr richEditHandle = IntPtr.Zero;

                            // 자식 윈도우들을 열거하여 RICHEDIT50W 컨트롤을 찾음
                            WindowEnumerator.EnumChildWindows(hWnd, (childHwnd, lP) =>
                            {
                                StringBuilder className = new StringBuilder(256);
                                WindowEnumerator.GetClassName(childHwnd, className, className.Capacity);

                                if (className.ToString() == "RICHEDIT50W")
                                {
                                    richEditHandle = childHwnd;
                                    return false; // 찾았으므로 중지
                                }
                                return true;
                            }, IntPtr.Zero);

                            // RICHEDIT50W 핸들을 찾으면 선택된 채팅방의 핸들값을 반환
                            if (richEditHandle != IntPtr.Zero)
                            {
                                selectedHandle = richEditHandle;
                            }
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);

            // RICHEDIT50W 핸들을 찾았으면 Form2로 전달하여 표시
            if (selectedHandle != IntPtr.Zero)
            {
                Form2 form2 = new Form2();
                form2.ShowRichEditHandle(selectedHandle);
                form2.Show();
            }
            else
            {
                MessageBox.Show("선택한 채팅방의 RICHEDIT50W 핸들을 찾을 수 없습니다.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            ListChatWindows();
        }
    }
}
