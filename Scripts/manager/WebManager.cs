using MoreMountains.CorgiEngine;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic; 
namespace shootstar
{


    public class WebManager : MonoBehaviour
    {
        [HideInInspector]public List<Property_Web> webs;
        [HideInInspector]public bool isInWeb;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(isInWeb)
            {
                foreach (var web in webs)
                {
                    if(web==null) continue;
                    if (web.isWebActive==false)
                    {
                        if (web.pauseWeb)
                        {
                            return;
                        }
                        if(web.gameObject.GetComponent<TerrainProperty>()!=null)
                            web.gameObject.GetComponent<TerrainProperty>().enabled=false;
                        //web.gameObject.GetComponent<GravityZone>().enabled=false;
                    }
                }
            }
            else if(!isInWeb)
            {
                foreach (var web in webs)
                {
                    if(web==null) continue;
                    if (web.pauseWeb)
                    {
                        return;
                    }
                    if (web.gameObject.GetComponent<TerrainProperty>() != null)
                        web.gameObject.GetComponent<TerrainProperty>().enabled=true;
                    //web.gameObject.GetComponent<GravityZone>().enabled=true;
                }
            }
        }
    }
}