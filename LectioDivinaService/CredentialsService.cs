using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LectioDivina.Model;
using System.IO;
using System.IO.Packaging;

using System.Threading.Tasks;
namespace LectioDivina.Service
{
    public interface ICredentialsService
    {
        Credentials Load();

        void Save(Credentials credentials);
    }

    public class CredentialsService: ICredentialsService
    {
        public const string AppDataSubfolder = "LectioDivina";
        public const string DataFilename = "LectioDivina_pwd.xml";

        public Credentials Load() 
        {
            Credentials credentials;
            try
            {
                EnsureDataFolder();

                using (var sr = new StreamReader(GetLocalFileName()))
                {
                    var s = sr.ReadToEnd();
                    credentials = SerializationHelper.Deserialize<Credentials>(s);
                }
            }
            catch (Exception)
            {
                credentials = new Credentials();
            }

            return credentials;
        }

        public void Save(Credentials credentials)
        {
            string xml;

            EnsureDataFolder();

            xml = SerializationHelper.Serialize(credentials);
            using (var sw = new System.IO.StreamWriter(GetLocalFileName()))
            {
                sw.WriteLine(xml);
            }

        }

        private void EnsureDataFolder()
        {
            if (!Directory.Exists(GetDataFolderName()))
                Directory.CreateDirectory(GetDataFolderName());
        }
        private string GetLocalFileName()
        {
            return Path.Combine(GetDataFolderName(), DataFilename);
        }

        private string GetDataFolderName()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppDataSubfolder);
        }
    }

    public class CredentialsValidator
    {
        public static Credentials UpdateEmailPwdIfMissing(Credentials credentials, ICredentialsService credentialsService)
        {
            if (String.IsNullOrEmpty(credentials.mailPwd))
            {
                PasswordInputBox passwordInputBox = new PasswordInputBox();
                string pwd = passwordInputBox.Show("proszę podać hasło do wysyłania wiadomości", "Wiadomości");
                credentials.mailPwd = pwd;
                credentialsService.Save(credentials);
            }
            return credentials;
        }

        public static Credentials UpdateUploadPwdIfMissing(Credentials credentials, ICredentialsService credentialsService)
        {
            if (String.IsNullOrEmpty(credentials.uploadPwd))
            {
                PasswordInputBox passwordInputBox = new PasswordInputBox();
                string pwd = passwordInputBox.Show("proszę podać hasło do wysyłania danych", "Upload danych");
                credentials.uploadPwd = pwd;
                credentialsService.Save(credentials);
            }
            return credentials;
        }
    }
}
