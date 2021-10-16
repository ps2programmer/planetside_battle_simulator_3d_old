using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSimulator
{
    public class UtilityFunctions
    {
    	public static System.Random random = new System.Random();

        public static bool LineOfSightExistsToTarget(Vector3 position1, Vector3 position2, LayerMask ignoreLayerMask) {
            Vector3 rayDirection = position2 - position1;
            float rayDistance = rayDirection.magnitude;
            bool noLineOfSight = Physics.Raycast(position1, rayDirection, rayDistance, ~ignoreLayerMask);
            return !noLineOfSight;
        }

        public static int RandInt(int start, int end) {
        	return random.Next(start, end);
        }
    }
}
