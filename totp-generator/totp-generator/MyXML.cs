using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace totp_generator
{
    class MyXML
    {
        private readonly String _path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\totp-generator-keys\\Accounts.xml";
        private ObservableCollection<Account> _listAccount;
        public ObservableCollection<Account> ListAccount
        {
            get { return _listAccount; }
            set { _listAccount = value; }
        }

        public MyXML()
        {
            _listAccount = new ObservableCollection<Account>();
        }

        private String ReadFile()
        {
            CheckFileExist();
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(_path))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            return sb.ToString();
        }

        public void AddElem(String name, String key)
        {
            CheckFileExist();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(this.ReadFile());
                XmlNode elemPath = doc.CreateNode(XmlNodeType.Element, "Key", doc.DocumentElement.NamespaceURI);
                elemPath.InnerText = key;
                XmlNode elemName = doc.CreateNode(XmlNodeType.Attribute, "name", doc.DocumentElement.NamespaceURI);
                elemName.Value = name;
                XmlNode elemPlaylist = doc.CreateNode(XmlNodeType.Element, "Account", doc.DocumentElement.NamespaceURI);
                elemPlaylist.AppendChild(elemPath);
                elemPlaylist.Attributes.SetNamedItem(elemName);
                XmlElement root = doc.DocumentElement;
                root.AppendChild(elemPlaylist);
                using (StreamWriter w = new StreamWriter(_path, false, Encoding.UTF8))
                {
                    w.WriteLine(doc.OuterXml);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error xml :" + e);
                MessageBox.Show(e.Message, "Error account", MessageBoxButton.OK);
            }
        }

        public void DeleteElem(String name, String key)
        {
            CheckFileExist();
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(this.ReadFile());
                XmlNode node = doc.SelectSingleNode("/Accounts/Account[@name='" + name + "'][Key='" + key + "']");
                Console.WriteLine("Deleting account : " + node.InnerXml);
                node.ParentNode.RemoveChild(node);
                doc.Save(_path);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error xml :" + e);
                MessageBox.Show(e.Message, "Error account", MessageBoxButton.OK);
            }
        }

        public void Refresh(ListBox listbox)
        {
            CheckFileExist();
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(_path);
                XmlNodeList nodes = xmlDoc.DocumentElement.SelectNodes("Account");
                _listAccount.Clear();
                foreach (XmlNode node in nodes)
                {
                    Account elem = new Account();
                    elem.Name = node.Attributes["name"].Value;
                    elem.Key = node.SelectSingleNode("Key").InnerText;
                    _listAccount.Add(elem);
                }

                listbox.ItemsSource = _listAccount;
            }
            catch (XmlException)
            {
                File.Delete(_path);
                this.Refresh(listbox);
            }
        }

        private void CheckFileExist()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\totp-generator-keys"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\totp-generator-keys");
            if (File.Exists(_path))
                return;
            new XDocument(new XElement("Accounts")).Save(_path);
        }
    }
}
