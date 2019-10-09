using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        List<string> urls = Parser.GetUrls("[\n  {\n    \"nodeId\": 0,\n    \"nodeType\": \"MasterNode\",\n    \"root\": {\n      \"root_analysis\": 2,\n      \"root_universal\": 3\n    },\n    \"next\": 1\n  },\n  {\n    \"elements\": [\n      {\n        \"bundleURL\": \"https://stackoverflow.com/questions/22058831/best-way-to-structure-a-parser-class\",\n        \"sound\": {\n          \"endTime\": 10.6875,\n          \"id\": \"k19hh7cx\",\n          \"name\": \"Applause.mp3\",\n          \"startTime\": 0,\n          \"volume\": 1\n        }\n      },\n      {\n        \"bundleURL\": \"https://firebasestorage.googleapis.com/v0/b/late-night-show.appspot.com/o/storyExport%2F-LqCB0YD13zlmNL06w1x%2F1570033655.json?alt=media&token=dcb0000c-5392-47b1-a73e-138d1e4711e7\",\n        \"character\": {\n          \"elements\": [\n            {\n              \"animation\": {\n                \"destination\": {\n                  \"position\": [\n                    0,\n                    0,\n                    -6\n                  ],\n                  \"rotation\": [\n                    0,\n                    0,\n                    0\n                  ]\n                },\n                \"endTime\": 5.0625,\n                \"loopCount\": 1,\n                \"name\": \"Disappearing Shader Effect\",\n                \"startTime\": 0.0625\n              },\n              \"id\": \"k19gg0d4\"\n            },\n            {\n              \"id\": \"k19gg0d5\",\n              \"line\": {\n                \"country\": \"English (US)\",\n                \"endTime\": 16.52297,\n                \"gender\": \"Standard Male\",\n                \"startTime\": 10.625,\n                \"text\": \"Thank you, thank you. Hello everyone. Welcome to the ibegoo late night show. \",\n                \"type\": \"Specific\",\n                \"voiceEffect\": \"None\",\n                \"voiceName\": \"en-US-Standard-C.mp3\",\n                \"voiceVolume\": 1\n              }\n            },\n            {\n              \"animation\": {\n                \"destination\": {\n                  \"position\": [\n                    0,\n                    0,\n                    0\n                  ],\n                  \"rotation\": [\n                    0,\n                    0,\n                    0\n                  ]\n                },\n                \"endTime\": 10.6625,\n                \"loopCount\": 2,\n                \"name\": \"Walking In Place\",\n                \"startTime\": 5.0625\n              },\n              \"id\": \"k19hdlq4\"\n            },\n            {\n              \"animation\": {\n                \"destination\": {\n                  \"position\": [\n                    0,\n                    0,\n                    0\n                  ],\n                  \"rotation\": [\n                    0,\n                    0,\n                    0\n                  ]\n                },\n                \"endTime\": 15.6125,\n                \"loopCount\": 1,\n                \"name\": \"Waving Hello\",\n                \"startTime\": 10.6625\n              },\n              \"id\": \"k19hdv9e\"\n            }\n          ],\n          \"endTime\": 20.0625,\n          \"id\": \"k19geugl\",\n          \"layer\": 3,\n          \"name\": \"SpaceshipTVHead\",\n          \"origin\": {\n            \"position\": [\n              0,\n              0,\n              -6\n            ],\n            \"rotation\": [\n              0,\n              0,\n              0\n            ]\n          },\n          \"startTime\": 0.0625,\n          \"type\": \"Character\",\n          \"model\": \"SpaceshipTVHead\"\n        }\n      }\n    ],\n    \"name\": \"SCENE 1\",\n    \"nodeId\": 1,\n    \"nodeType\": \"SceneNode\",\n    \"next\": -1,\n    \"startTime\": 0,\n    \"endTime\": 20.0625\n  }\n]");
        urls = Parser.GetUrlsContaining(urls, "firebase");
        if (urls.Count != 0)
        {
            foreach (string url in urls)
            {
                Debug.Log(url);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
