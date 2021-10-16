using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSimulator.SoldierScripts;
using UnityEngine.UI;

namespace BattleSimulator.Core
{
    public class Spawner : MonoBehaviour
    {
        // PUBLIC
        public Soldier soldierPrefab;

        public InputField numberOfTRToSpawnInputField;
        public InputField numberOfNCToSpawnInputField;
        public InputField numberOfVSToSpawnInputField;
        public Toggle randomizeSoldierSkillToggle;
        public GameObject skillSettings;
        public InputField skillTRInputField;
        public InputField skillNCInputField;
        public InputField skillVSInputField;

        [Tooltip("The number of TR to spawn at the beginning of the game.")]
        public int numberOfTRToSpawn;
        [Tooltip("The skill of all TR soldiers. If the randomize skill checkbox is checked it will override this.")]
        [Range(0.0f, 1.0f)]
        public float skillTR;
        [Tooltip("The number of NC to spawn at the beginning of the game.")]
        public int numberOfNCToSpawn;
        [Tooltip("The skill of all NC soldiers. If the randomize skill checkbox is checked it will override this.")]
        [Range(0.0f, 1.0f)]
        public float skillNC;
        [Tooltip("The number of VS to spawn at the beginning of the game.")]
        public int numberOfVSToSpawn;
        [Tooltip("The skill of all VS soldiers. If the randomize skill checkbox is checked it will override this.")]
        [Range(0.0f, 1.0f)]
        public float skillVS;
        [Tooltip("If checked, this randomizes the skill of each soldier spawned to a value between 0.0 and 1.0. If not checked, it will set the skill equal to the skill value you set above.")]
        public bool randomizeSkill;
        [Tooltip("The probability of a soldier moving randomly applied to all soldiers on TR.")]
        [Range(0.0f, 1.0f)]
        public float probabilityOfMovingRandomlyTR;
        [Tooltip("The probability of a soldier moving randomly applied to all soldiers on NC.")]
        [Range(0.0f, 1.0f)]
        public float probabilityOfMovingRandomlyNC;
        [Tooltip("The probability of a soldier moving randomly applied to all soldiers on VS.")]
        [Range(0.0f, 1.0f)]
        public float probabilityOfMovingRandomlyVS;
        [Tooltip("If checked, this randomizes the probability of moving randomly for each soldier.")]
        public bool randomizeProbabilityOfMovingRandomly;

        void Awake() {
        	numberOfTRToSpawn = 0;
        	numberOfNCToSpawn = 0;
        	numberOfVSToSpawn = 0;
        	randomizeSkill = false;
        	skillTR = 0.8f;
        	skillNC = 0.8f;
        	skillVS = 0.8f;
        }

        public void OnNumberOfTRToSpawnInputFieldEdited() {
        	string fieldText = numberOfTRToSpawnInputField.text;
        	if (fieldText != "") {
        		numberOfTRToSpawn = Int32.Parse(fieldText);
    		} else {
    			numberOfTRToSpawn = 0;
    		}
        }

        public void OnNumberOfNCToSpawnInputFieldEdited() {
        	string fieldText = numberOfNCToSpawnInputField.text;
        	if (fieldText != "") {
        		numberOfNCToSpawn = Int32.Parse(fieldText);
    		} else {
    			numberOfNCToSpawn = 0;
    		}
        }

        public void OnNumberOfVSToSpawnInputFieldEdited() {
        	string fieldText = numberOfVSToSpawnInputField.text;
        	if (fieldText != "") {
        		numberOfVSToSpawn = Int32.Parse(fieldText);
    		} else {
    			numberOfVSToSpawn = 0;
    		}
        }

        public void OnSkillTRInputFieldEdited() {
        	string fieldText = skillTRInputField.text;
        	if (fieldText != "") {
        		skillTR = float.Parse(fieldText);
    		} else {
    			skillTR = 0.0f;
    		}
        }

        public void OnSkillNCInputFieldEdited() {
        	string fieldText = skillNCInputField.text;
        	if (fieldText != "") {
        		skillNC = float.Parse(fieldText);
    		} else {
    			skillNC = 0.0f;
    		}
        }

        public void OnSkillVSInputFieldEdited() {
        	string fieldText = skillVSInputField.text;
        	if (fieldText != "") {
        		skillVS = float.Parse(fieldText);
    		} else {
    			skillVS = 0.0f;
    		}
        }

        public void OnRandomizeSkillToggled() {
        	if (randomizeSoldierSkillToggle.isOn) {
        		skillSettings.SetActive(false);
        	} else {
        		skillSettings.SetActive(true);
        	}
        }

        public void OnSpawnSoldiersButtonPressed()
        {
        	randomizeSkill = randomizeSoldierSkillToggle.isOn;

        	if (!randomizeSkill) {
        		if (skillTR > 1.0f) {
        		skillTR = 1.0f;
	        	}
	        	if (skillNC > 1.0f) {
	        		skillNC = 1.0f;
	        	}
	        	if (skillVS > 1.0f) {
	        		skillVS = 1.0f;
	        	}
	        	if (skillTR < 0.0f) {
	        		skillTR = 0.0f;
	        	}
	        	if (skillNC < 0.0f) {
	        		skillNC = 0.0f;
	        	}
	        	if (skillVS < 0.0f) {
	        		skillVS = 0.0f;
	        	}
        	}

            for (int i = 0; i < numberOfTRToSpawn; i++) {
                Soldier newSoldier = Instantiate(soldierPrefab, Vector3.zero, Quaternion.identity) as Soldier;
                newSoldier.faction = "TR";
                newSoldier.deathCount = 0;
                if (randomizeSkill) {
                    newSoldier.skill = (float)UtilityFunctions.random.NextDouble();
                } else {
                    newSoldier.skill = skillTR;
                }
            }
            for (int i = 0; i < numberOfNCToSpawn; i++) {
                Soldier newSoldier = Instantiate(soldierPrefab, Vector3.zero, Quaternion.identity) as Soldier;
                newSoldier.faction = "NC";
                newSoldier.deathCount = 0;
                if (randomizeSkill) {
                    newSoldier.skill = (float)UtilityFunctions.random.NextDouble();
                } else {
                    newSoldier.skill = skillNC;
                }
            }
            for (int i = 0; i < numberOfVSToSpawn; i++) {
                Soldier newSoldier = Instantiate(soldierPrefab, Vector3.zero, Quaternion.identity) as Soldier;
                newSoldier.faction = "VS";
                newSoldier.deathCount = 0;
                if (randomizeSkill) {
                    newSoldier.skill = (float)UtilityFunctions.random.NextDouble();
                } else {
                    newSoldier.skill = skillVS;
                }
            }
        }
    }
}