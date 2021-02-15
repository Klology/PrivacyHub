using System;
using System.Collections.Generic;

namespace PrivacyHub
{
	public class Device
	{

		private string availability;
		private string caption;
		private string classGuid;
		private string compatibleID;
		private string configManagerErrorCode;
		private string configManagerUserConfig;
		private string creationClassName;
		private string description;
		private string deviceID;
		private string errorCleared;
		private string errorDescription;
		private string hardwareID;
		private string installDate;
		private string lastErrorCode;
		private string manufacturer;
		private string name;
		private string pNPClass;
		private string pNPDeviceID;
		private string powerManagementCapabilities;
		private string powerManagementSupported;
		private string present;
		private string service;
		private string status;
		private string statusInfo;
		private string systemCreationClassName;
		private string systemName;

		public string Availability { get; }
		public string Caption { get; }
		public string ClassGuid { get; }
		public string CompatibleID { get; }
		public string ConfigManagerErrorCode { get; }
		public string ConfigManagerUserConfig { get; }
		public string CreationClassName { get; }
		public string Description { get; }
		public string DeviceID { get; }
		public string ErrorCleared { get; }
		public string ErrorDescription { get; }
		public string HardwareID { get; }
		public string InstallDate { get; }
		public string LastErrorCode { get; }
		public string Manufacturer { get; }
		public string Name { get; }
		public string PNPClass { get; }
		public string PNPDeviceID { get; }
		public string PowerManagementCapabilities { get; }
		public string PowerManagementSupported { get; }
		public string Present { get; }
		public string Service { get; }
		public string Status { get; }
		public string StatusInfo { get; }
		public string SystemCreationClassName { get; }
		public string SystemName { get; }

		

		//default constructor
		public Device()
		{
			availability = "";
			caption = "";
			classGuid = "";
			compatibleID = "";
			configManagerErrorCode = "";
			configManagerUserConfig = "";
			creationClassName = "";
			description = "";
			deviceID = "";
			errorCleared = "";
			errorDescription = "";
			hardwareID = "";
			installDate = "";
			lastErrorCode = "";
			manufacturer = "";
			name = "";
			pNPClass = "";
			pNPDeviceID = "";
			powerManagementCapabilities = "";
			powerManagementSupported = "";
			present = "";
			service = "";
			status = "";
			statusInfo = "";
			systemCreationClassName = "";
			systemName = "";
		}

		//parameterized constructor. Takes managementBaseObject
		public Device(System.Management.ManagementBaseObject usbDevice)
		{
			List<string> deviceProperties = new List<string>();

			foreach (var property in usbDevice.Properties)
			{
				//Cycles through properties of usbDevice and stores values in a list. If an exception occurs, the item in list is set as null
				try
				{
					deviceProperties.Add(property.Value.ToString());
				}
				catch(Exception e)
                {
					deviceProperties.Add(null);
				}
				finally
                {

                }
			}

			//assignments are made using data from list. Looks disgusting but was the only way I could think to make the assignments. Order of list will be consistent from usbDevice properties
			availability = deviceProperties[0];
			caption = deviceProperties[1];
			classGuid = deviceProperties[2];
			compatibleID = deviceProperties[3];
			configManagerErrorCode = deviceProperties[4];
			configManagerUserConfig = deviceProperties[5];
			creationClassName = deviceProperties[6];
			description = deviceProperties[7];
			deviceID = deviceProperties[8];
			errorCleared = deviceProperties[9];
			errorDescription = deviceProperties[10];
			hardwareID = deviceProperties[11];
			installDate = deviceProperties[12];
			lastErrorCode = deviceProperties[13];
			manufacturer = deviceProperties[14];
			name = deviceProperties[15];
			pNPClass = deviceProperties[16];
			pNPDeviceID = deviceProperties[17];
			powerManagementCapabilities = deviceProperties[18];
			powerManagementSupported = deviceProperties[19];
			present = deviceProperties[20];
			service = deviceProperties[21];
			status = deviceProperties[22];
			statusInfo = deviceProperties[23];
			systemCreationClassName = deviceProperties[24];
			systemName = deviceProperties[25];
			
		}

		public string toString()
		{
			return ("----------------DEVICE---------------" +
					"\nAvailability: " + availability +
					"\nCaption: " + caption +
					"\nClassGuid: " + classGuid +
					"\nCompatibleID: " + compatibleID +
					"\nConfigManagerErrorCode: " + configManagerErrorCode +
					"\nConfigManagerUserConfig: " + configManagerUserConfig +
					"\nCreationClassName: " + creationClassName +
					"\nDescription: " + description +
					"\nDeviceID: " + deviceID +
					"\nErrorCleared: " + errorCleared +
					"\nErrorDescription: " + errorDescription +
					"\nHardwareID: " + hardwareID +
					"\nInstallDate: " + installDate +
					"\nLastErrorCode: " + lastErrorCode +
					"\nManufacturer: " + manufacturer +
					"\nName: " + name +
					"\nPNPClass: " + pNPClass +
					"\nPNPDeviceID: " + pNPDeviceID +
					"\nPowerManagementCapabilities: " + powerManagementCapabilities +
					"\nPowerManagementSupported: " + powerManagementSupported +
					"\nPresent: " + present +
					"\nService: " + service +
					"\nStatus: " + status +
					"\nStatusInfo: " + statusInfo +
					"\nSystemCreationClassName: " + systemCreationClassName +
					"\nSystemName: " + systemName +
					"\n----------------END---------------");
		}
	}
}