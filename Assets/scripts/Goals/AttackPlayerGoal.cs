using System.Linq;
using Interfaces;
using UnityEngine;
using Utils;

namespace Goals
{
    public class AttackPlayerGoal : IGoal
    {
        private readonly GameObject _relatedObject;
        private readonly IShooter _shooter;
        private readonly IRotatable _rotatable;

        private Vector2 _directionToPlayer;
        
        public AttackPlayerGoal(GameObject relatedObject)
        {
            if (!relatedObject.TryGetComponent(out IShooter shooter))
            {
                throw new System.Exception("Object must have IShooter interface");
            }

            if (!relatedObject.TryGetComponent(out IRotatable rotatable))
            {
                throw new System.Exception("Object must have IRotatable interface");
            }

            _relatedObject = relatedObject;
            _shooter = shooter;
            _rotatable = rotatable;
        }
        
        public bool IsAvailable()
        {
            Vector3 currentPosition = _relatedObject.transform.position;
            
            GameObject player = ObjectsFinder.GetNearestWithTag(currentPosition, 10.0f, "Player");
            if (player == null)
            {
                return false;
            }

            Vector2 direction = player.transform.position - currentPosition;
            float distance = direction.magnitude;
            _directionToPlayer = VectorUtils.RoundToCardinal(direction);
            direction = _directionToPlayer * distance;
            Debug.DrawLine(currentPosition, currentPosition + (Vector3) direction, Color.yellow);

            RaycastHit2D[] hits = Physics2D.RaycastAll(currentPosition, direction, distance);
            return !hits.Any(hit => hit.collider.CompareTag("Wall")) &&
                    hits.Any(hit => hit.collider.CompareTag("Player"));
        }

        public void Execute()
        {
            Vector2 facingDirection = _relatedObject.transform.right;
            if (facingDirection != _directionToPlayer)
            {
                _rotatable.RotateTowards(_directionToPlayer, 360.0f);
            }
            else
            {
                _shooter.Shoot();
            }
        }
    }
}