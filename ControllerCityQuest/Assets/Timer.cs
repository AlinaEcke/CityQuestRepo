
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
 
public class Timer : MonoBehaviour {

void OnTriggerEnter(Collider other) {

        if (other.gameObject.name.Equals("playerAvatar")) {
            saveTimeToFile();
        }
}

public void saveTimeToFile() {

    File.AppendAllText("CompletionTimes.txt", Time.realtimeSinceStartup + Environment.NewLine);
}


}
