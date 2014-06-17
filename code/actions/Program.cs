using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Phone.Test.Utilities;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using Microsoft.Phone.Test.MediaApps.MusicVideo.Library;
using Microsoft.Phone.Test.Utilities.Wnf;
using Microsoft.Phone.Test.Shell.StartX.AreaLibrary;
using Microsoft.Phone.Test.Security.SecurityModel;
using System.Threading;
using System.Threading.Tasks;
//using Windows.Phone.PersonalInformation.CalendarRT;
//using PhoneInternal.Experiences.Calendar;
using Windows.Foundation;

namespace Actions
{
    static class Utilities
    {
        public static string TryGetText(this Tile tile, PropertyName propertyName)
        {
            try
            {
                return tile.GetText(propertyName).Content;
            }
            catch { return null; }
        }
    }

    class Program
    {
        static bool audioConnected = false;
        //static IList<CalendarStore> calendarStores;
        static Timer bluetoothConnectTimer;
        static Dictionary<string, List<Timer>> busyTimers;

        static object locker = new object();
        public static void WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
            string textToWrite = String.Format(format, arg) + Environment.NewLine;

            lock (locker)
            {
                File.AppendAllText(@"c:\data\test\bin\actions.log", textToWrite);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception)
            {
                Exception exception = e.ExceptionObject as Exception;
                File.AppendAllText(@"c:\data\test\bin\exception.log", exception.Message);
            }
        }

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            File.Delete(@"c:\data\test\bin\actions.log");

            busyTimers = new Dictionary<string, List<Timer>>();
            if (args.Length > 0)
            {
                if (args[0].Equals("queryBT", StringComparison.CurrentCultureIgnoreCase))
                {
                    Microsoft.Phone.Test.Bluetooth.Common.AreaLibrary.BtcmObserver.RegisterTheCallback();
                    Microsoft.Phone.Test.Bluetooth.Common.AreaLibrary.BtcmObserver.PrintDeviceStates();

                    var cpl = new Microsoft.Phone.Test.Bluetooth.Common.AreaLibrary.BluetoothCpl();
                    cpl.PrintPeripheralList();
                }
                else if (args[0].Equals("queryStart", StringComparison.CurrentCultureIgnoreCase))
                {
                    int i = 0;
                    foreach (var tile in AreaLibraryManager.TilesPage.GetAllTiles())
                    {
                        Console.Write("{0})\t{1}", i++, ApplicationManager.GetApplicationTitleFromProductId(tile.ProductId));
                        //Console.WriteLine("\t{0}: {1}: {2}?{3}", tile.TileId, tile.TaskId, tile.TaskUri, tile.TaskParameters);


                        var props = tile.GetPropertyNames();
                        if (props.Contains(PropertyName.Title))
                        {
                            Console.Write(" - {0}", tile.GetPropertyValue(PropertyName.Title));
                        }
                        if (props.Contains(PropertyName.Content))
                        {
                            Console.Write(" - {0}", tile.GetPropertyValue(PropertyName.Content));
                        }
                        //Console.WriteLine(tile.TryGetText(PropertyName.Content));
                        //tile.GetPropertyNames().ForEach(prop => Console.WriteLine("\t{0}:\t{1}", prop, tile.TryGetText(prop)));
                        Console.WriteLine();
                        var uri = String.Format("{0}?{1}", tile.TaskUri, tile.TaskParameters);
                        Console.WriteLine(uri);
                    }
                }
                else
                {
                    int index = int.Parse(args[1]);
                    var tile = AreaLibraryManager.TilesPage.GetAllTiles()[index];
                    var uri = String.Format("{0}?{1}", tile.TaskUri, tile.TaskParameters);
                    new Timer(o =>
                    {
                        Microsoft.Phone.Test.NavigationModel.NavigationManager.LaunchSession(uri);
                    }, null, (long)(DateTime.Parse(args[0]) - DateTime.Now).TotalMilliseconds, Timeout.Infinite);
                    Console.ReadLine();
                }
            }
            else
            {
                //System.Threading.Thread.Sleep(20000);
                WnfState bluetoothState = null;
                try
                {
                    bluetoothState = WnfState.GetState(0xa3bc0875, 0x0992022f); // WNF_BLTH_BLUETOOTH_STATUS
                    audioConnected = AudioIsConnected(bluetoothState.QueryData());
                    bluetoothState.StateChanged += bluetoothState_StateChanged;
                }
                catch (Exception ex)
                {
                    WriteLine(ex.ToString());
                    return;
                }

                    bluetoothConnectTimer = new Timer(o =>
                    {
                        //Console.WriteLine("AudioIsConnected: {0}", AudioIsConnected(bluetoothState.QueryData()));
                        if (!AudioIsConnected(bluetoothState.QueryData()))
                        {
                            WriteLine("Connecting...");
                            int result = Connect(0x272E46ACC);
                            if (result != 0)
                            {
                                ErrorUtilities.ThrowWin32Exception(result, "Connect");
                            }
                        }
                    }, null, 0, 10000);

                //Task setupCalendarTask = SetupAllCalendarEvents();
                //setupCalendarTask.Wait();

                Console.ReadLine();

                bluetoothConnectTimer.Dispose();
            }
        }

        #region Ringer

        //static bool IsDuringAppointment(DateTimeOffset time, Appointment appointment)
        //{
        //    return time > appointment.StartTime && time < (appointment.StartTime + appointment.Duration);
        //}

        //static async Task SetupCalendarEvents(CalendarStore store)
        //{
        //    var aqo = new AppointmentQueryOptions();
        //    aqo.StartDate = DateTimeOffset.Now;
        //    aqo.EndDate = DateTimeOffset.Now.AddHours(2);

        //    var appointments = await store.CreateAppointmentQuery(aqo).GetAppointmentsAsync();
        //    var busyAppointments = appointments.Where(a => a.BusyStatus == Windows.ApplicationModel.Appointments.AppointmentBusyStatus.Busy).ToList();

        //    busyAppointments.ForEach(appt => WriteLine(appt.Subject));

        //    foreach (var appointment in busyAppointments)
        //    {
        //        if (!busyAppointments.Any(a => IsDuringAppointment(appointment.StartTime, a)))
        //        {
        //            ScheduleRingerChange(store.Name, appointment.StartTime, true);
        //        }

        //        var endTime = appointment.StartTime + appointment.Duration;
        //        if (!busyAppointments.Any(a => IsDuringAppointment(endTime, a)))
        //        {
        //            ScheduleRingerChange(store.Name, endTime, false);
        //        }
        //    }
        //}

        //static async Task SetupAllCalendarEvents()
        //{
        //    TypedEventHandler<object, CalendarChangedEventArgs> handler = async (object sender, CalendarChangedEventArgs args) =>
        //    {
        //        var calendar = await CalendarManager.GetCalendarAsync(args.ItemId);
        //        WriteLine(calendar.Name);

        //        var store = await CalendarManager.GetStoreAsync(calendar.StoreId);
        //        busyTimers[store.Name].Clear();

        //        SetRinger(false);
        //        await SetupCalendarEvents(store);
        //    };

        //    calendarStores = await CalendarManager.CreateCalendarStoreQuery().GetCalendarStoresAsync();
        //    foreach (var store in calendarStores)
        //    {
        //        Object temp = store;
        //        var icalendarStore = (temp as ICalendarStoreInternal);
        //        icalendarStore.CalendarChanged += handler;

        //        var calendarStore = temp as CalendarStore;
        //        busyTimers.Add(calendarStore.Name, new List<Timer>());
        //        await SetupCalendarEvents(calendarStore);
        //    }
        //}

        static void ScheduleRingerChange(string calendar, DateTimeOffset triggerTime, bool busy)
        {
            WriteLine("{0}: Setting Busy={1}", triggerTime, busy);

            if (triggerTime < DateTime.Now)
            {
                SetRinger(busy);
            }
            else
            {
                var silentTimer = new Timer(o =>
                {
                    SetRinger(busy);
                }, null, (long)(triggerTime - DateTime.Now).TotalMilliseconds, Timeout.Infinite);

                busyTimers[calendar].Add(silentTimer);
            }
        }

        static void SetRinger(bool busy)
        {
            int ringer = busy ? 1 : 3;
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\EventSounds\Sounds", "RingerVibrateState", ringer);
        }

        #endregion

        #region Bluetooth/Music

        static bool AudioIsConnected(uint state)
        {
            return (state & 0x20) == 0x20;
        }

        static void bluetoothState_StateChanged(object sender, WnfStateChangedEventArgs e)
        {
            WriteLine("0x{0:X}", e.Value);

            if (AudioIsConnected(e.Value)) // BGS_AUDIO_CONNECTED
            {
                if (!audioConnected)
                {
                    audioConnected = true;
                    WriteLine("Connected, playing music");
                    PlayMusic();
                }
            }
            else
            {
                if (audioConnected)
                {
                    audioConnected = false;
                    WriteLine("Disconnected");
                }
            }
        }

        static void PlayMusic()
        {
            // Spotify - starred
            Microsoft.Phone.Test.NavigationModel.NavigationManager.LaunchSession("app://10F2995D-1F82-4203-B7FA-46DDBD07A6E6/_default?action=DeeplinkPlaylistUri&spotifyUri=spotify:user:palenshus:starred");

            // Spotify - Retrograde
            //Microsoft.Phone.Test.NavigationModel.NavigationManager.LaunchSession("app://10F2995D-1F82-4203-B7FA-46DDBD07A6E6/_default?action=DeeplinkPlaylistUri&spotifyUri=spotify:user:palenshus:playlist:29QXqJpJhH3gQx5lbanIKQ");

            // Xbox Music
            //Microsoft.Phone.Test.NavigationModel.NavigationManager.LaunchSession("app://5B04B775-356B-4AA0-AAF8-6491FFEA5630/_PlayLocalMusicContainer?zmi=0x7000fd1");

            // Quantum of Solace
            //Microsoft.Phone.Test.NavigationModel.NavigationManager.LaunchSession("app://5B04B775-356B-4AA0-AAF8-6491FFEA5630/_PlayLocalMusicContainer?zmi=0x6000019");
        }

        [DllImport("BTDll.dll")]
        static extern int Connect(ulong btAddr);

        #endregion
    }
}
