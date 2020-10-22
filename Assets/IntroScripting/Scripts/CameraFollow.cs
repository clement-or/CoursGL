using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public enum FollowType
    {
        LookAt,
        FollowBehind,
        FollowStatic
    }

    public FollowType followType = FollowType.FollowStatic;

    private void Update()
    {
        switch (followType)
        {
            case FollowType.FollowBehind:
                FollowBehind();
                break;
        }
    }

    private void FollowBehind()
    {
        
    }
}