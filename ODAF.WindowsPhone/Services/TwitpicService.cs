using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Windows.Media.Imaging;

namespace ODAF.WindowsPhone.Services
{
    /// <summary>
    /// An helper class to send photos to Twitpic
    /// </summary>
    public class TwitpicService
    {
        /// <summary>
        /// An event to be notified when the image has been uploaded to Twitpic.
        /// </summary>
        public event EventHandler<TwitpicImageUploadedEventArgs> TwitpicImageUploaded;

        private const string TWITPIC_UPLADO_API_URL = "http://twitpic.com/api/upload";
        private const string TWITPIC_UPLOAD_AND_POST_API_URL = "http://twitpic.com/api/uploadAndPost";

        private byte[] bytes;
        private string username;
        private string password;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bytes">The binary data of the image to upload.</param>
        /// <param name="username">The Twitter login of the user.</param>
        /// <param name="password">The Twitter password of the user.</param>
        public TwitpicService(byte[] bytes, string username, string password)
        {
            this.bytes = bytes;
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// The principal method of this class to upload a photo to Twitpic.
        /// </summary>
        public void UploadPhoto() 
        { 
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://twitpic.com/api/upload");
            request.ContentType = "application/x-www-form-urlencoded"; 
            request.Method = "POST"; 
            request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
        }

        private void GetRequestStreamCallback(IAsyncResult asynchronousResult)      
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState; 
                string encoding = "iso-8859-1";                // End the operation					  
                Stream postStream = request.EndGetRequestStream(asynchronousResult); 
                string boundary = Guid.NewGuid().ToString();                
                request.ContentType = string.Format("multipart/form-data; boundary={0}",boundary);      
                string header = string.Format("--{0}", boundary);            
                string footer = string.Format("--{0}--", boundary);            
                StringBuilder contents = new StringBuilder();           
                contents.AppendLine(header);
                string fileHeader = String.Format("Content-Disposition: file; name=\"{0}\";filename=\"{1}\"; ", "media", "testpic.png");
                string fileData = Encoding.GetEncoding(encoding).GetString(this.bytes, 0, this.bytes.Length);    
                contents.AppendLine(fileHeader);       
                contents.AppendLine(String.Format("Content-Type: {0};", "image/png"));      
                contents.AppendLine();       
                contents.AppendLine(fileData);   
                contents.AppendLine(header);     
                contents.AppendLine(String.Format("Content-Disposition: form-data;name=\"{0}\"", "username"));     
                contents.AppendLine();
                contents.AppendLine(this.username);   
                contents.AppendLine(header);   
                contents.AppendLine(String.Format("Content-Disposition: form-data;name=\"{0}\"", "password"));   
                contents.AppendLine();     
                contents.AppendLine(this.password);   
                contents.AppendLine(footer);                // Convert the string into a byte array.      
                byte[] byteArray =Encoding.GetEncoding(encoding).GetBytes(contents.ToString());                // Write to the request stream.   
                postStream.Write(byteArray, 0, contents.ToString().Length); 
                postStream.Close();                // Start the asynchronous operation to get the response      
                request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);   
            }
            catch (Exception ex)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show(ex.Message);
                });
            } 
        }

        private void GetResponseCallback(IAsyncResult asynchronousResult)      
        {        
            try          
            {            
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;     
                // End the operation          
                HttpWebResponse response =(HttpWebResponse)request.EndGetResponse(asynchronousResult);   
                Stream streamResponse = response.GetResponseStream();          
                StreamReader streamRead = new StreamReader(streamResponse);            
                string responseString = streamRead.ReadToEnd();           
                XDocument doc = XDocument.Parse(responseString);             
                XElement rsp = doc.Element("rsp");          
                string status = rsp.Attribute(XName.Get("status")) != null ? rsp.Attribute(XName.Get("status")).Value : rsp.Attribute(XName.Get("stat")).Value;         
                streamResponse.Close();             
                streamRead.Close();             
                response.Close();
                if (TwitpicImageUploaded != null)
                {
                    TwitpicImageUploaded(this, new TwitpicImageUploadedEventArgs() { TwitpicImageUrl = rsp.Element("mediaurl").Value });
                }
            }           
            catch (Exception ex)         
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Twitpic Error : " + ex.Message);
                });
            }  
        }
    }

    public class TwitpicImageUploadedEventArgs : EventArgs
    {
        public string TwitpicImageUrl { get; set; }
    }

}
