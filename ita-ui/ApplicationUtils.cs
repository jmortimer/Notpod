﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iTunesAgent.Domain;
using iTunesAgent.UI.Properties;
using System.Windows.Forms;
using iTunesAgent.UI.Configuration;

namespace iTunesAgent.UI
{
    public class ApplicationUtils
    {
        #if DEBUG
        public static readonly string APP_CONFIG_PATH = "ApplicationConfiguration.xml";
        public static readonly string DEVICES_CONFIG_PATH = "Devices.xml";
        public static readonly string PATTERN_CONFIG_PATH = "Pattern.xml";
        #else
        public static readonly string APP_CONFIG_PATH = GetUserAppDataPath() + "\\ApplicationConfiguration.xml";
        public static readonly string DEVICES_CONFIG_PATH = GetUserAppDataPath() + "\\Devices.xml";
        public static readonly string PATTERN_CONFIG_PATH = GetUserAppDataPath() + "\\Pattern.xml";
        #endif

        /*!
         * Populates the ModelRepository with configuration from file.
         */
        public static ModelRepository LoadModelRepository(ConfigurationChecker configurationChecker)
        {
            ModelRepository repo = new ModelRepository();

            LoadApplicationConfiguration(configurationChecker, repo);
            LoadStoredDevices(repo);
            LoadStoredPatterns(repo);

            return repo;
        }
        
        static void LoadStoredPatterns(ModelRepository repo)
        {
            // If there are no current stored devices config, simply add a new DeviceCollection.
            if (!File.Exists(PATTERN_CONFIG_PATH))
            {
                repo["patterns"] = new PatternCollection();
                return;
            }

            Stream read;
            try
            {
                read = new FileStream(PATTERN_CONFIG_PATH, FileMode.Open);
            }
            catch (Exception ex)
            {
                throw new IOException("Unable to load stored patterns.", ex);
            }

            try
            {
                repo.Deserialize("patterns", typeof(iTunesAgent.Domain.PatternCollection), read);
            }
            finally
            {

                if (read != null)
                {
                    read.Close();
                }
            }
        }

        private static void LoadStoredDevices(ModelRepository repo)
        {

            // If there are no current stored devices config, simply add a new DeviceCollection.
            if (!File.Exists(DEVICES_CONFIG_PATH))
            {
                repo["devices"] = new DeviceCollection();
                return;
            }

            Stream read;
            try
            {
                read = new FileStream(DEVICES_CONFIG_PATH, FileMode.Open);
            }
            catch (Exception ex)
            {

                throw new IOException("Unable to load stored devices configuration.", ex);
            }

            try
            {
                repo.Deserialize("devices", typeof(iTunesAgent.Domain.DeviceCollection), read);
            }
            finally
            {

                if (read != null)
                {
                    read.Close();
                }
            }
        }

        private static void LoadApplicationConfiguration(ConfigurationChecker configurationChecker, ModelRepository repo)
        {
            Stream read;
            try
            {
                configurationChecker.CheckConfiguration();
                read = new FileStream(APP_CONFIG_PATH, FileMode.Open);

            }
            catch (Exception ex)
            {

                throw new IOException("Unable to load application configuration.", ex);
            }

            try
            {
                repo.Deserialize("appconfig", typeof(iTunesAgent.Domain.Configuration), read);
            }
            finally
            {

                if (read != null)
                {
                    read.Close();
                }
            }
        }

        public static string GetUserAppDataPath()
        {
            return Application.UserAppDataPath;
        }

        internal static Stream GetDeviceConfigurationStream(FileMode mode)
        {
            Stream stream = new FileStream(DEVICES_CONFIG_PATH, mode);
            return stream;
        }
    }
}
