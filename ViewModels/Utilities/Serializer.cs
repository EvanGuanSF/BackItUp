using BackItUp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            try
            {
                IsSerializerIdle = false;

                if (!IsSerializerIdle)
                {
                    stream = File.Open(AppDomain.CurrentDomain.BaseDirectory + @"BackItUpBackupConfig.dat", FileMode.Create);
                    binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, backupCollection.ToList());
                    stream.Close();

                    IsSerializerIdle = true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SaveConfigToFile: " + e.Message);
            }
        }

        /// <summary>
        /// Deserialize the BackupInfo collection in .dat format from the default config path and add to collection.
        /// </summary>
        public static ObservableCollection<BackupItem> LoadConfigFromFile()
        {
            try
            {
                IsSerializerIdle = false;
                // If we have a config file, then load and return it. Otherwise, return an empty collection.
                if (!IsSerializerIdle && File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"BackItUpBackupConfig.dat"))
                {
                    stream = File.Open(AppDomain.CurrentDomain.BaseDirectory + @"BackItUpBackupConfig.dat", FileMode.Open);
                    try
                    {
                        // Create a new collection.
                        ObservableCollection<BackupItem> backupCollection;

                        // Open the file, deserialize contents, close the stream, and return the new collection.
                        binaryFormatter = new BinaryFormatter();
                        List<BackupItem> streamData = (List<BackupItem>)binaryFormatter.Deserialize(stream);
                        backupCollection = new ObservableCollection<BackupItem>(streamData);
                        stream.Close();

                        IsSerializerIdle = true;
                        return backupCollection;
                    }
                    catch
                    {
                        // If there was an error in the backup data, just load a new BackupInfo collection instead.
                        stream.Close();
                        IsSerializerIdle = true;
                        return new ObservableCollection<BackupItem>();
                    }
                }
                else
                {
                    IsSerializerIdle = true;
                    return new ObservableCollection<BackupItem>();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SaveConfigToFile: " + e.Message);
                IsSerializerIdle = true;
                return new ObservableCollection<BackupItem>();
            }
        }
    }
}
