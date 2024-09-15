using System.Collections.Generic;
using Godot;

namespace Hanabushii;
interface IEntity {

    /*
        Things that we will be able to do with entities:
            1. Get their position (they will set/update it)
            2. Get a list of their hitboxes
            3. Apply a force to them (gravity, pushes, etc)
            4. Get the current force thats applied to them
    */
    public Vector3 getPosition();
    public List<CollisionPolygon2D> getCollision();
    public Vector3 applyForce();
    public Vector3 getForce();

}