using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
	public class soldier_movement_tests
	{
	    [Test]
	    public void soldier_movement_testsSimplePasses()
	    {
	    }

	    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
	    // `yield return null;` to skip a frame.
	    [UnityTest]
	    public IEnumerator soldier_movement_testsWithEnumeratorPasses()
	    {
	        // Use the Assert class to test conditions.
	        // Use yield to skip a frame.
	        yield return null;
	    }
	}
}
