using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {

	void ApplyDamage(GameObject damager, DamageType type, float power = 1.0f);
}
