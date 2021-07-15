# unity-destruction
 
An open-source script to destroy objects realistically in Unity3D.
 
Forked from William-BL added new features and got to a stage where the example should work.
 
## Features

- Make stuff break up, on impact with other stuff!
- Make stuff break up when there's nothing underneath supporting it!
- Make stuff break up for no reason whatsoever!
- Make stuff explode!
- Make stuff make sounds when it breaks up!
- Make stuff make particles when it breaks up!
- Things can also be configured to only partially break apart, depending on impact velocity.
- Includes an example game where you can throw a ball at a cube. It's more fun than it sounds.
--New Features
-Fixed script and example scenes. The last commit had examples that didn’t work right
-New way to work with particles so they are generated when pieces collide.
-Added ability to have stuff explode on break.
-The game example scene can now explode objects with a right-click.

## How to use

- Make an unbroken and a broken version of your object. I recommend using Blender's [cell fracture](https://duckduckgo.com/?q=blender+cell+fracture) feature. Ensure that the unbroken version is the first object in the blender scene and that the scene only contains meshes (eg no lights).
-Add mesh to your unity scene. You should have a parent game object with the broken and unbroken meshes as children.
-Add a MeshCollider with the convex setting on and a Rigidbody with a mass of 1 (can be set higher if desired) to each broken piece mesh.
-Select the parent game object, add a RigidBody with a mass of 2500(again can be changed if necessary) also add a BoxCollider which encases the whole unbroken mesh. Now add the Destruction script('Destruction/Assets/Scripts/Destruction.cs') set breakage multiplier to zero and strength to 10000(this stops the old partial break feature from working, you can experiment later if you desire)
-now your object is ready to be broken
 
--Explode on Break
This new feature will make an object exploded when enough force has been used to collide with it.
--To use
-Goto the destruction script, set explode on break to true then experiment with the radius and power.
 
--New particle technique
-I’ve added a different way of generating particles on breakage. The old way(which still works if you prefer) would generate 1 big particle cloud on the initial break, it’s fine if everything falls apart but not so good if you want to break big objects off with multiple hits. My new technique makes small particles spawn when a piece collides with something.
--To use
-Create a prefab with just a particle system and configure as desired. Add the DestroyOnParticlesEnd script('Destruction/Assets/Scripts/DestroyOnParticlesEnd.cs’).
-Now add the SpawnParticlesOnCollision script('Destruction/Assets/Scripts/SpawnParticlesOnCollision.cs’) and assign your prefab, you can also assign a different material that will replace the material in the prefab if you desire, this is to prevent you from having to make a new prefab just to change the color.
 
--You can programmatically break an object with the Destruction.Break() to break or Destruction.BreakWithExplosiveForce(float, float) methods.
 
- Mess with the settings until you get the desired break effect.
- For examples, see 'Destruction/Assets/example-game/example-game'.
 
 
By cypherstalker