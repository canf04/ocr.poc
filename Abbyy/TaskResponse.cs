using System;
using System.Xml;
using System.Xml.Serialization;

namespace OCR.POC.Abbyy
{
    /*
     * id="c3187247-7e81-4d12-8767-bc886c1ab878" 
        registrationTime="2012-02-16T06:42:09Z" 
        statusChangeTime="2012-02-16T06:42:09Z" 
        status="Queued" 
        filesCount="1" 
        credits="0" 
        estimatedProcessingTime="1" 
        description="Image.JPG to .pdf"
     */
    public class TaskResponse
    {
        [XmlAttribute]
        public string id { get; set; }
        public DateTime registrationTime { get; set; }
        public DateTime statusChangeTime { get; set; }
        public string status { get; set; }
        public int filesCount { get; set; }
        public int credits { get; set; }
        public int estimatedProcessingTime { get; set; }
        public string description { get; set; }
        [XmlAttribute]
        public string resultUrl { get; set; }
    }
}
