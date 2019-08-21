using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace CAPDemo.Controllers
{
    public class PublishController : Controller
    {
        private readonly ICapPublisher _capBus;
        private string ConnectionString = "SERVER=192.168.4.100;DATABASE=otopt;UID=root;PASSWORD=8858;port=3307";

        public PublishController(ICapPublisher capPublisher)
        {
            _capBus = capPublisher;
        }

        //不使用事务
        [Route("~/without/transaction")]
        public IActionResult WithoutTransaction()
        {
            _capBus.Publish("services.show.time", DateTime.Now);

            return Ok();
        }

        //Ado.Net 中使用事务，自动提交
        [Route("~/adonet/transaction")]
        public IActionResult AdonetWithTransaction()
        {
            using (var connection = new MySqlConnection(ConnectionString))
            {
                using (var transaction = connection.BeginTransaction(_capBus, autoCommit: true))
                {
                    //业务代码
                    string a = null;
                    decimal b = Convert.ToDecimal(a);


                    _capBus.Publish("services.show.time", DateTime.Now);
                }
            }
            return Ok();
        }

        ////EntityFramework 中使用事务，自动提交
        //[Route("~/ef/transaction")]
        //public IActionResult EntityFrameworkWithTransaction([FromServices]AppDbContext dbContext)
        //{
        //    using (var trans = dbContext.Database.BeginTransaction(_capBus, autoCommit: true))
        //    {
        //        //业务代码

        //        _capBus.Publish("xxx.services.show.time", DateTime.Now);
        //    }
        //    return Ok();
        //}

        [CapSubscribe("services.show.time")]
        public async Task CheckReceivedMessage(DateTime time)
        {
            Console.WriteLine(time);
            await Task.CompletedTask;
        }
    }
}