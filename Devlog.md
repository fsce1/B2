# Character Controller
## Feb 20th - Feb 26th
The first thing I had to do was to create a first-person character controller. I need to be able to walk, crouch, sprint, jump, lean and potentially lie prone.

After doing some searching online, I found an example Github Repo that mimicks the Source Engine movement code. This was the code used in CS:2, Half-Life 2 and other Valve games.
https://github.com/Olezen/UnitySourceMovement
I also found some short videos that explained this code in very simple terms, and helped me bugfix.
https://www.youtube.com/watch?v=v3zT3Z5apaM
I read through the movement code, and translated the parts I needed into my own project. I missed out on things such as ladder movement and swimming, as the map doesnt include either.
The camera and player movement happens in the `Player.cs` file.
```cs
private void CalculateGroundMovement()
{
	//wishDir is direction you wish to move in, or the vector directly resulting from your key presses
    Vector3 wishDir = Vector3.Normalize(inputMovement.y * transform.forward + inputMovement.x * transform.right);

    float wishSpeed = maxSpeed / 100;

    //currentSpeed is not actually the current speed, but the dot product of the current velocity and the direction your arrow keys are pressing.
    //This is what enables air-strafing, allowing the player to bunnyhop.
    float currentSpeed = Vector3.Dot(velocity, wishDir);
    
    float addSpeed = wishSpeed - currentSpeed;
    float accelSpeed = Mathf.Min(accel * Time.deltaTime * wishSpeed, addSpeed);
    
    //Add speed in wish direction 
    velocity.x += accelSpeed * wishDir.x;
    velocity.z += accelSpeed * wishDir.z;
    
	//Calculate the deceleration
    float speed = velocity.magnitude;
    float drop = speed * decel * Time.deltaTime;
    float newSpeed = Mathf.Max(speed - drop, 0);
    if (speed > 0) newSpeed /= speed;
    //Apply deceleration
    velocity.x *= newSpeed;
    velocity.z *= newSpeed;
}
```
This creates a much higher quality character movement controller than a lot of other solutions for Unity, as it has smooth acceleration and deceleration.
```cs
    private void CalculateView()
    {
	    //Delta input for mouse movement
        inputView *= sensitivity / 100;

        cameraAngles += inputView;
        //Clamp the Y axis to +-90 so you can't do "backflips"
        cameraAngles.y = Mathf.Clamp(cameraAngles.y, -90, 90);
		//Rotate the entire player on the Y axis, and only the camera holder on the X axis.This means the entire player object is always looking forward, but the player does not glitch out when you look up or down as vertical rotation is only done on the camera.
        Quaternion camRot = Quaternion.AngleAxis(-cameraAngles.y, Vector3.right);
        Quaternion playerRot = Quaternion.AngleAxis(cameraAngles.x, Vector3.up);
        
        camHolder.localRotation = camRot;
        transform.localRotation = playerRot;
    }
```
# Weapon Sway
## Feb 27th - Mar 3rd
Once I had completed the movement controller, it was time to work on the weapon controller. I searched online for some tutorials on first person weapons in Unity, and found this very helpful tutorial series.
https://www.youtube.com/playlist?list=PLW3-6V9UKVh2T0wIqYWC1qvuCk2LNSG5c
First, I worked on weapon sway, which is the gun's movement when you look around quickly, are currently walking, etc. 
```cs
void CalculateWeaponRot()
{
	//if you are aiming down sights, scale each component of the sway down by individual amounts
    float _movementScaler = 1;
    float _swayScaler = 1;
    if (isAiming)
    {
        _movementScaler = movementRotScaler;
        _swayScaler = swayRotScaler;
    }
	//Most weapon sway is done using Vector3.SmoothDamp
	//This smoothly moves the object to the target position, with a smoothness set by the user
    weaponRotation.x += GameManager.GM.player.accumulatedInputView.y * swayAmount;
    weaponRotation.y += -GameManager.GM.player.accumulatedInputView.x * swayAmount;
    weaponRotation = Vector3.SmoothDamp(weaponRotation, Vector3.zero, ref weaponRotationVelocity, swaySmoothing);
    newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, weaponRotation, ref newWeaponRotationVelocity, swayResetSmoothing);
    newWeaponRotation.z = newWeaponRotation.y * 0.75f;

    movementRotation.z = -movementSwayAmount * GameManager.GM.player.inputMovement.x;
    movementRotation = Vector3.SmoothDamp(movementRotation, Vector3.zero, ref movementRotationVelocity, movementSwaySmoothing);
    newMovementRotation = Vector3.SmoothDamp(newMovementRotation, movementRotation, ref newMovementRotationVelocity, movementSwaySmoothing);
    //Apply the new positions multiplied by the aiming scaler
    wpnRot += newWeaponRotation * _swayScaler + newMovementRotation * _movementScaler;
}
```
The tutorial series helped me with starting the weapon system, however it had many issues and I had to write quite a lot of my own code. For example, the youtuber used an animation clip for walking, but I wanted a more procedural system, where you can change the step speed and how far it steps side to side. It also allowed me to slightly randomise the step locations to add some realism.
```cs
void FixedUpdate(){
	//Simple iterator acts as a timer
	if (curWalkLifetime < walkLifetime * walkLifetimeScaler)
	{
	    curWalkLifetime += 1;
	}
	else
	{
		//Swap foot
	    curWalkLifetime = 0;
	    rightFoot = !rightFoot;
	}
}
```

```cs
    void CalculateWalk()
    {
	    //Scale down if aiming down sights
        float _walkScaler = 1;
        if (isAiming) _walkScaler = walkScaler;

		//if in the second half of the step, the target is the rest position
        Vector3 target = Vector3.zero;
        if (GameManager.GM.player.velocity.magnitude > 0.01f)
        {
	        //if you are in the first half of the step, target is moved down and towards the foot that is currently stepping
            if (curWalkLifetime < (walkLifetime * walkLifetimeScaler) / 2)
            {
                float lateralVelocity = new Vector2(GameManager.GM.player.velocity.x, GameManager.GM.player.velocity.z).magnitude;
                target.y -= Random.Range(stepDownAmount.x, stepDownAmount.y) * lateralVelocity;
                float sideAmount = Random.Range(stepSideAmount.x, stepSideAmount.y);
                //Move to left or right depending on boolean set in FixedUpdate()
                if (rightFoot) target.x += sideAmount * lateralVelocity;
                else target.x -= sideAmount * lateralVelocity;
            }
        }
        //Apply movement towards target
        walkMove = Vector3.SmoothDamp(walkMove, target, ref walkMoveVelocity, walkMoveSmoothing);
        newWalkMove = Vector3.SmoothDamp(newWalkMove, walkMove, ref newWalkMoveVelocity, walkMoveSmoothing);
        
        wpnPos += newWalkMove * _walkScaler;
        wpnRot += new Vector3(newWalkMove.y, newWalkMove.x, -newWalkMove.x * 1.5f) * stepRotScaling;
    }
```
I also wrote a similar method for breathing, with some randomness to increase realism.
Once I was done with that, I also needed to allow the player to aim down the sights of their gun. This is where I started to create the actual gun logic (shooting, reloading, recoil, etc.) Each firearm has 2 Vector3s called `restPos` and `aimPos`. These are the 2 positions of the gun at rest and while you are aiming down sights. They are different for each gun, so is stored in the `firearmInfo` class along with stats such as recoil, fire rate, fire modes and magazine size.
# Weapon
## Mar 4th
I then added shooting mechanics.
```cs
void FixedUpdate(){
	//Single-Fire just calls the Shoot() method as soon as it is pressed
    if (canShoot && fullAutoHeld && curFireMode == FireMode.fullAuto && !isReloading && roundsInMag > 0)
    {
        Shoot();
    }
}
 void Shoot()
 {
	 //Instantiate round at barrel point
     GameObject roundObj = Instantiate(roundPrefab, barrelPoint.position, transform.rotation);
     roundObj.GetComponent<Round>().firearmFiredFrom = this;

     AddRecoil();
     canShoot = false;
     roundsInMag -= 1;
     sustainedRecoilAdd += info.sustainedRecoilAdd;

     if (muzzleFlash != null)
     {
         muzzleFlash.transform.localEulerAngles = new(muzzleFlash.transform.localEulerAngles.x, muzzleFlash.transform.localEulerAngles.y, UnityEngine.Random.Range(0, 360));
         muzzleFlash.SetActive(true);
         Invoke(nameof(ResetMuzzleFlash), Time.deltaTime * 2.5f);
     }

     if (roundsInMag >= 0)
     {
	     //Invoke() calls a method after a set period of time
	     //In this circumstance, ResetShot() sets canShoot back to true after a single rpm interval (converted into seconds from rounds per minute)
         Invoke(nameof(ResetShot), 1/ (info.roundsPerMinute/60));
     }
```
I also added a button (V by default), to change the fire mode from single fire to full-auto. Once all of the shooting was finished, recoil was next.
Recoil is done in a very similar way to weapon sway, by adding rotation to the gun using `Vector3.SmoothDamp()`. There are 4 ways that recoil affects you, vertical rotation, horizontal rotation, camera rotation and lateral movement. Each shot's recoil is slightly randomized as well, to make spray patterns different every time.


# Map Design
At this point, I knew that the debug scene I had created in order to test movement needed an update to further iterate on my game concept. I started making a larger scene that would become the first area in the game: "Clifftown". Originally a small town in Sujusterea, it was taken over by military forces and turned into a stronghold defending important intelligence. With a large cliff overlooking the town, long range combat is inevitable, and you'll want to bring an optic.



I knew that I wanted to have a realistic bullet physics, including travel time and 