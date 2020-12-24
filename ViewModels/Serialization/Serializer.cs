﻿using BackItUp.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace BackItUp.ViewModels.Serialization
{
    /// <summary>
    /// This class reads and writes the BackupInfo collection from and to a file, respectively.
    /// </summary>
    public static class Serializer
    {
        // Flag for enabling/disabling other serializtion jobs.
        public static bool IsSerializerIdle { get; private set; } = true;
        private static Stream stream;
        private static BinaryFormatter binaryFormatter;

        /// <summary>
        /// Serializes the passed in collection to .dat format and save in the default config path.
        /// </summary>
        /// <param name="backupCollection"></param>
        public static void SaveConfigToFile(ObservableCollection<BackupItem> backupCollection)
        {
            IsSerializerIdle = false;

            if (!IsSerializerIdle)
            {
                stream = File.Open(System.AppDomain.CurrentDomain.BaseDirectory + @"BackItUpBackupConfig.dat", FileMode.Create);
                binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, backupCollection.ToList());
                stream.Close();

                IsSerializerIdle = true;
            }
        }

        /// <summary>
        /// Deserialize the BackupInfo collection in .dat format from the default config path and add to collection.
        /// </summary>
        public static ObservableCollection<BackupItem> LoadConfigFromFile()
        {
            IsSerializerIdle = false;
            // If we have a config file, then load and return it. Otherwise, return an empty collection.
            if (!IsSerializerIdle && File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + @"BackItUpBackupConfig.dat"))
            {

                // Create a new collection.
                ObservableCollection<BackupItem> backupCollection;

                // Open the file, deserialize contents, close the stream, and return the new collection.
                binaryFormatter = new BinaryFormatter();
                stream = File.Open(System.AppDomain.CurrentDomain.BaseDirectory + @"BackItUpBackupConfig.dat", FileMode.Open);
                List<BackupItem> streamData = (List<BackupItem>)binaryFormatter.Deserialize(stream);
                backupCollection = new ObservableCollection<BackupItem>(streamData);
                stream.Close();

                IsSerializerIdle = true;
                return backupCollection;
            }
            else
            {
                IsSerializerIdle = true;
                return new ObservableCollection<BackupItem>();
            }
        }
    }
}
