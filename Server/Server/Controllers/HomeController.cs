using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Server.Models;
using Server.Others;

namespace Server.Controllers
{

    public enum DeviceType { Signalization = 1, Door, Conditioner, Sensor }
    public enum Days { Monday = 1, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday }
    public static class Times
    {
        public static int year = 1;
        public static int month = 1;
        public static int sec = 0;
    }
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public string Rec(int i)
        {
            TcpClient Client = new TcpClient();
            try
            {
                Client.Connect(IPAddress.Parse("192.168.1.4"), 2200);
            }
            catch
            {
                return "Cannot connect to remote host!";
            }

            Socket Sock = Client.Client;

            string data = "YaZakonectilsaaaaaaaaaaaaaaaaaaa";
            Sock.Send(Encoding.ASCII.GetBytes(data));

            Sock.Close();
            Client.Close();
            return "Hello World From Bro=" + i.ToString();

        }


        public ActionResult Index()
        {


            using (var context = new DevicesEntities())
            {
                var result = (from ent in context.Device
                              where ent.UserName == User.Identity.Name
                              select ent).ToList();
                ViewBag.lol = result.Count;
                return View(result);
            }

        }
        public ActionResult CreateShedule(int DeviceSerial)
        {
            Time time = new Time();
            time.DeviceSerial = DeviceSerial;
            return View(time);
        }
        [HttpPost]
        public ActionResult CreateShedule(Time time)
        {

            using (var context = new DevicesEntities())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        context.Time.Add(time);
                        context.SaveChanges();
                        //return RedirectToAction("Schedule");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, ex);
                }
                return View(time);
            }
        }

        public ActionResult DeleteShedule(int id)
        {
            using (var context = new DevicesEntities())
            {
                var employeeDelete = (from entity in context.Time
                                      where entity.Id == id
                                      select entity).First();
                return View(employeeDelete);
            }
        }


        [HttpPost]
        public ActionResult DeleteShedule(int id, FormCollection collection)
        {

            using (var context = new DevicesEntities())
            {
                var employeeDelete = (from entity in context.Time
                                      where entity.Id == id
                                      select entity).First();
                try
                {
                    context.Time.Attach(employeeDelete);
                    context.Time.Remove(employeeDelete);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(employeeDelete);
                }
            }
        }
        public ActionResult Schedule(int DeviceSerial)
        {

            using (var context = new DevicesEntities())
            {
                var device = (from entity in context.Device
                              where entity.DeviceSerial == DeviceSerial
                              select entity).First();

                ViewBag.DeviceSerial = device.DeviceSerial;

                var deviceTimes = (from entity in context.Time
                                   where entity.DeviceSerial == device.DeviceSerial
                                   select entity).ToList();
                return View(deviceTimes);
            }

        }





        //[HttpPost]
        //public ActionResult Schedule(int id,Time time,string Day)
        //{
        //    //Time newTime = new Time();
        //    //newTime.From.Value.Day = (int)Enum.Parse(typeof(Days), Day);
        //    //time.From.Value.DayOfWeek = (int)Enum.Parse(typeof(Days),Day);
        //    using (var context = new DevicesEntities())
        //    {
        //        try
        //        {
        //            if (ModelState.IsValid)
        //            {
        //                context.Time.Add(time);
        //                context.SaveChanges();
        //                return RedirectToAction("Index");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ModelState.AddModelError(String.Empty, ex);
        //        }
        //        return View(time);
        //    }

        //}




        public ActionResult TurnOnOff(int DeviceSerial)
        {
            using (var context = new DevicesEntities())
            {
                var device = (from entity in context.Device
                              where entity.DeviceSerial == DeviceSerial
                              select entity).First();
                if (device.State == 0)
                {
                    device.State = 1;
                }
                else
                {
                    device.State = 0;
                }
                UpdateModel(device);
                context.SaveChanges();
                TcpClient Client = new TcpClient();
                Client.Connect(IPAddress.Parse("192.168.1.4"), 2200);

                Socket Sock = Client.Client;
                Sock.Send(MySerialize.serialize(device, typeof(Device)));

                Sock.Close();
                Client.Close();

                return RedirectToAction("Index", "Home");
            }


        }
        public ActionResult setTiming(int DeviceSerial)
        {

            using (var context = new DevicesEntities())
            {
                var device = (from entity in context.Device
                              where entity.DeviceSerial == DeviceSerial
                              select entity).First();

                var times = (from entity in context.Time
                             where entity.DeviceSerial == DeviceSerial
                             select entity).ToList();
                Packet packet = new Packet();
                packet.device = device;
                packet.time = times;

                TcpClient Client = new TcpClient();
                Client.Connect(IPAddress.Parse("192.168.1.4"), 2200);

                Socket Sock = Client.Client;
                Sock.Send(MySerialize.serialize(packet, typeof(Packet)));

                Sock.Close();
                Client.Close();

                return RedirectToAction("Index", "Home");
            }

        }
        public void setTconst()
        {

        }



        public string AddDevice(int DeviceSerial, int Type, int State)
        {

            using (var context = new DevicesEntities())
            {

                Device device = new Device();
                device.DeviceSerial = DeviceSerial;
                device.Type = Type;
                device.State = State;
                device.UserName = "Kalter";
                try
                {
                    context.Device.Add(device);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }

                return "Device" + device.DeviceSerial + "Connected";

            }
        }

        //public string AddSensor(int DeviceSerial, int Type, int State,int tconst,int t)
        //{


        //}
        [HttpGet]
        public string GetState()
        {
            using (var context = new DevicesEntities())
            {
                var result = (from ent in context.Device
                              where ent.UserName == User.Identity.Name
                              select ent).ToList();
                String states = "";
                for (int i = 0; i < result.Count; i++)
                {
                    states += result[i].State.ToString();
                }

                return states;
            }
        }

        [HttpGet]
        public string OnDeviceLoad()
        {
            using (var context = new DevicesEntities())
            {
                var result = (from ent in context.Device
                              where ent.UserName == User.Identity.Name
                              select ent).ToList();

                string row="" ;

                for (int i = 0; i < result.Count; i++)
                {
                    row += "<tr><td>"
                                +result[i].DeviceSerial+"</td><td>"
                                +result[i].Type+"</td><td>"
                                +result[i].State+"</td></tr>";
                }

                return row;
            }

        }




        public string sendDevices()
        {
            using (var context = new DevicesEntities())
            {
                var devicesToSend = (from ent in context.Device
                                     select ent).ToList();
                TcpClient Client = new TcpClient();

                Client.Connect(IPAddress.Parse("192.168.1.4"), 2200);


                Socket Sock = Client.Client;


                Sock.Send(MySerialize.serialize(devicesToSend, typeof(List<Device>)));

                Sock.Close();
                Client.Close();
                return devicesToSend.Count + "Devices Sended";


            }
        }
        public string AlarmSignalization(int DeviceSerial)
        {
            ViewBag.dev = DeviceSerial;
            ViewBag.type = "AlarmSignalization";

            //Redirect("http://localhost:81/Server/Home");
            return Redirect("http://localhost:81/Server/Home").Url;
        }
        public string AlarmSensor(int DeviceSerial)
        {
            ViewBag.dev = DeviceSerial;
            ViewBag.type = "AlarmSensor";
            return "AlarmSensor";
        }
        public string ChangeState(int DeviceSerial, int state)
        {
            using (var context = new DevicesEntities())
            {
                var device = (from entity in context.Device
                              where entity.DeviceSerial == DeviceSerial
                              select entity).First();


                device.State = state;

                UpdateModel(device);
                context.SaveChanges();
                return "New State is " + device.State;
            }
        }
        public string ChangeTemperature(int DeviceSerial, int temperature)
        {
            ViewBag.temperature = temperature;
            ViewBag.dev = DeviceSerial;
            ViewBag.type = "ChangeTemperature";
            return "ChangeTemperature";
        }





    }
}
