using System;
using System.Collections.Generic;

namespace PrivacyHub
{
	public class Device
	{

		private string name;
		private string pNPClass;
		private string pNPDeviceID;
		private string pNPDeviceIDSubstring;
		private bool hasSearchableSubstring;

		public string Name { get; }
		public string PNPClass { get; }
		public string PNPDeviceID { get; }
		public string PNPDeviceIDSubstring { get { return pNPDeviceIDSubstring; } }
		public bool HasSearchableSubstring { get { return hasSearchableSubstring; } }

		//parameterized constructor. Takes managementBaseObject
		public Device(System.Management.ManagementBaseObject usbDevice)
		{
			List<string> deviceProperties = new List<string>();

			foreach (var property in usbDevice.Properties)
			{

				System.Diagnostics.Debug.WriteLine("Test to see CompatibleID string: " + usbDevice.GetPropertyValue("CompatibleID").ToString());

				//Cycles through properties of usbDevice and stores values in a list. If an exception occurs, the item in list is set as null
				try
				{
                    if (property.Value != null) {
						deviceProperties.Add(property.Value.ToString());
					} else {
						deviceProperties.Add(null);
					}
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
			name = deviceProperties[15];
			pNPClass = deviceProperties[16];
			pNPDeviceID = deviceProperties[17];

			hasSearchableSubstring = AuthenticateSubstring();

		}

		public string toString()
		{
			return ("----------------DEVICE---------------" +
					"\nName: " + name +
					"\nPNPClass: " + pNPClass +
					"\nPNPDeviceID: " + pNPDeviceID +
					"\nPNPDeviceIDSubstring: " + pNPDeviceIDSubstring +
					"\n----------------END---------------");
		}

		//determines if the device has a proper PNPDeviceID that we can extract a searchable substring from
		public bool AuthenticateSubstring()
        {
			char[] charArrayID = pNPDeviceID.ToCharArray();
			int numBraces = 0;
			List<char> pnpSubstring = new List<char>();

			for(int i = 0; i < pNPDeviceID.Length; i++)	
            {
				if(charArrayID[i] == '{')	//searching for '{' to indicate if PNPDeviceID is in proper form. We need the second occurence of '{'
                {
					numBraces++;

					if (numBraces == 2)		//We are at the second occurence of '{'
					{
						int j = 0;
						while (charArrayID[i] != '}')  //add elements of substring to a list until we've reached the end of the substring
						{
							pnpSubstring.Add(charArrayID[i]);
							i++;
						}

						pnpSubstring.Add('}');

						//convert list form of substring into proper string. Print toString of Device
						string temp = new string(pnpSubstring.ToArray());
						pNPDeviceIDSubstring = temp;
						System.Diagnostics.Debug.WriteLine(toString());
						return true;
                    }

                }
            }

			return false;
        }
	}
}