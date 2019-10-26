﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Npgsql;

namespace OpenReviewConferenceManagementSoftware.utils
{
    static class DatabaseConnection
    {
        /// <summary>
        /// Creates a connectiong between the application and the Amazon RDS test database
        /// Note:
        ///     - Only have one connection open at a time and send one query per connection.
        ///       Not following this may lead to "randomness" in ability to perform queries.
        /// </summary>
        /// <returns>NpgsqlConnection to db if successful, else null.</returns>
        public static NpgsqlConnection EstablishConnection()
        {
            try
            {
                NpgsqlConnection dbConnection = new NpgsqlConnection("Server=" + dbConfig.host +
                    ";User Id=" + dbConfig.username +
                    ";Password=" + dbConfig.password +
                    ";Database=" + dbConfig.dbname +
                    ";Port=" + dbConfig.port + ";");
                return dbConnection;
            }
            catch
            {
                return null;
            }
        }
    }
}