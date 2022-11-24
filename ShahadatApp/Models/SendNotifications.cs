using ShahadatApp.Hubs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace ShahadatApp.Models
{

    public static class NotificaionService
    {
        static readonly string connString = "Data Source=WANHOST; User id = admin; password = new555; Initial Catalog=ShahadatApp;";

        internal static SqlCommand command = null;
        internal static SqlDependency dependency = null;


        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <returns></returns>
        public static string GetNotification()
        {
            try
            {

                var messages = new List<Notification>();
                using (var connection = new SqlConnection(connString))
                {

                    connection.Open();
                    //// Sanjay : Alwasys use "dbo" prefix of database to trigger change event
                    using (command = new SqlCommand("SELECT [NotificationID],[Status],[Message],[ExtraColumn] FROM [dbo].[Notification]", connection))
                    {
                        command.Notification = null;

                        if (dependency == null)
                        {
                            dependency = new SqlDependency(command);
                            dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                        }

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        var reader = command.ExecuteReader();
                        //Notification notifi = new Notification();-

                        while (reader.Read())
                        {
                            messages.Add(item: new Notification
                            {
                                NotificationID = (int)reader["NotificationID"],
                                Status = reader["Status"] != DBNull.Value ? (string)reader["Status"] : "",
                                Message = reader["Message"] != DBNull.Value ? (string)reader["Message"] : "",
                                ExtraColumn = reader["ExtraColumn"] != DBNull.Value ? (string)reader["ExtraColumn"] : ""
                            });
                        }
                    }

                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(messages);
                return json;

            }
            catch (Exception)
            {

                return null;
            }


        }

        private static void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (dependency != null)
            {
                dependency.OnChange -= dependency_OnChange;
                dependency = null;
            }
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.Send("تم إضافة الطلب بنجاح");
            }
        }
    }
}