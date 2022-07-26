using UnityEngine;
using PathCreation;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class Follower : MonoBehaviour
{
    private SplinePlacer splinePlacer;
    private PathCreator pathCreator { get => splinePlacer.pathCreator; }

    public EndOfPathInstruction endOfPathInstruction;

    public float Speed { get => splinePlacer.MinionSpeed; }

    float distanceTravelled;

    void Start()
    {
        if (splinePlacer != null && pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            pathCreator.pathUpdated += OnPathChanged;

            OnPathChanged();
        }
    }

    void Update()
    {
        if (splinePlacer != null && pathCreator != null)
        {
            distanceTravelled += Speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            //transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        }
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }

    public void SetPath(SplinePlacer splinePlacer)
    {
        this.splinePlacer = splinePlacer;
    }
}
