using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductivityUnit : Unit
{
    [SerializeField] private float productivityMultiplier =2f;

    private ResourcePile currentPile;
    protected override void BuildingInRange()
    {
        if (currentPile == null)
        {
            ResourcePile pile = m_Target as ResourcePile;

            if (pile != null)
            {
                currentPile = pile;
                currentPile.ProductionSpeed *= productivityMultiplier;
            }
        }
    }

    private void Reset()
    {
        if (currentPile != null)
        {
            currentPile.ProductionSpeed /= productivityMultiplier;
            currentPile = null;
        }
    }

    public override void GoTo(Vector3 position)
    {
        Reset();
        base.GoTo(position);
    }

    public override void GoTo(Building target)
    {
        Reset();
        base.GoTo(target);
    }
}
