using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable {

	bool ApplyDamage(GameObject damager, DamageType type, float power = 1.0f);
}
