using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
  public GameController gameController;
  public CPU cpu;
  public Pathfinder pathfinder;
  public List<string> knownDangers = new List<string>();
  public List<float> knownDangerWeights = new List<float>();

  // Start is called before the first frame update
  void Start(){
    gameController=GameObject.Find("GameController").GetComponent<GameController>();
    cpu = gameObject.GetComponent<CPU>();
    pathfinder = gameObject.GetComponent<Pathfinder>();
  }

  // Update is called once per frame
  void Update(){

  }

  public void changeMind(){

  }

  public void witnessDanger(string danger, float weight){
    int i = knownDangers.IndexOf(danger);
    float weight2 = weight;
    if (i>-1){
      weight2 = (knownDangerWeights[i]+weight)*.5f;
    }
    learnDanger(danger, weight2);
  }

  public void learnDanger(string danger, float weight){
    int i = knownDangers.IndexOf(danger);
    if (i>-1){
      knownDangers.RemoveAt(i);
      knownDangerWeights.RemoveAt(i);
    }
    knownDangers.Add(danger);
    knownDangerWeights.Add(weight);
  }
}
