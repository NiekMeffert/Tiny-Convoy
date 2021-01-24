using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{
  public float waitPerChar;
  GameController gameController;
  Text subHolder;
  float subDisappears;
  bool[] flags = new bool[16];
  GameObject lastTotem;

  // Start is called before the first frame update
  void Start(){
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    subHolder = gameObject.GetComponent<Text>();
  }

  // Update is called once per frame
  void Update(){
    if (Time.unscaledTime>subDisappears-.4f) subHolder.text="";
    if (gameController.totem!=null && lastTotem==null) lastTotem = gameController.totem;
    //Level 0: Greeting
    if (flags[0]==false){
      setText("Red: Hello small robot! You're online! This is good I think. I think? Yes! I am. And so are you. Which is good, I think.");
      flags[0]=true;
    }
    //Level 0.1: Assemble
    if (flags[0]==true && flags[1]==false && Time.unscaledTime>subDisappears){
      setText("Red: You first need to put yourself together. Those things next to you are upgrades. Click on them--or yourself--to go into Upgrade Mode.");
      flags[1]=true;
    }
    //Level 0.2: How to Assemble
    if (flags[1]==true  && flags[2]==false && gameController.mode==2){
      setText("Red: Drag parts onto yourself, off yourself, or up and down to move them around. When you're done, Upgrade yourself!");
      flags[2]=true;
    }
    //Level 1: Plants
    if (flags[2]==true && flags[3]==false && gameController.mode==1 && Time.unscaledTime>subDisappears){
      setText("Red: Yum! These plants provide power. We should make sure our batteries don't get too low. And don't lose me!");
      flags[3]=true;
    }
    //Level 2: Big bots
    if (gameController.totemLevel==2 && flags[4]==false && Time.unscaledTime>subDisappears){
      setText("Red: Do you hear that? Those big robots scare me. I have a Scanner to watch for them, but it's not very good. Hopefully we'll find better Upgrades farther along.");
      flags[4]=true;
    }
    //Changing
    if (lastTotem!=null && flags[5]==false && gameController.totem!=lastTotem && Time.unscaledTime>subDisappears){
      setText("Red: Whoa! That was weird. I think while we're meshed, our lead bot is going to change from time to time.");
      flags[5]=true;
    }
    //Level 3: Upgrades again
    if (gameController.totemLevel==3 && flags[6]==false && Time.unscaledTime>subDisappears){
      setText("Red: There's a lot of junk out here. I'll bet some of it still works. Be on the lookout for Upgrades!");
      flags[6]=true;
    }
    //Level 4: ???
    if (gameController.totemLevel==4 && flags[7]==false && Time.unscaledTime>subDisappears){
      setText("Red: Do you ever think about boys? ...Sorry, I meant, ''buoys.'' I don't know what either one it. I just want to know if you think about them.");
      flags[7]=true;
    }
    //Level 5: New friend
    if (gameController.totemLevel==5 && flags[8]==false && Time.unscaledTime>subDisappears){
      setText("Red: If we find another CPU, I'll bet we could build a new friend out of parts. Listen for the startup chime.");
      flags[8]=true;
    }
    //Level 6: Charging
    if (gameController.totemLevel==6 && flags[9]==false && Time.unscaledTime>subDisappears){
      setText("Red: Hey, I wonder if we can give each other power. What happens if you click on my battery?");
      flags[9]=true;
    }
    //Level 7: Dangers
    if (gameController.totemLevel==7 && flags[10]==false && Time.unscaledTime>subDisappears){
      setText("Red: There's something about this place I don't like. You don't suppose there are dangers other than big robots out here, do you?");
      flags[10]=true;
    }
    //Level 8: End
    if (gameController.totemLevel==8 && flags[11]==false && Time.unscaledTime>subDisappears){
      setText("Red: I probably shouldn't say this, but we're out of new content.");
      flags[11]=true;
    }
  }

  void setText(string txt){
    subDisappears = (txt.Length * waitPerChar) + Time.unscaledTime;
    subHolder.text=txt;
  }
}
