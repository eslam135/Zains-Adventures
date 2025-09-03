using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TocuhingChecker : MonoBehaviour
{
    [SerializeField] private ContactFilter2D contactFilter;
    [SerializeField] private float groundDistance = 0.05f;

    private CapsuleCollider2D coll;
    private List<RaycastHit2D> raycastHit2Ds;
    private Animator anim;

    [SerializeField] private bool _isGrounded = false;
    public bool IsGrounded
    {
        get { return _isGrounded; }
        private set
        {
            _isGrounded = value;
            anim.SetBool("isGrounded", value);
        }
    }
    private void Awake()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
        raycastHit2Ds = new List<RaycastHit2D>();
    }

    void FixedUpdate()
    {
        IsGrounded = coll.Cast(Vector2.down, contactFilter, raycastHit2Ds, groundDistance) > 0;
    }
}
