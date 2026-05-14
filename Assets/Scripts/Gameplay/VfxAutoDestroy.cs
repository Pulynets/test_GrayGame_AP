using UnityEngine;

// Універсальний самознищувач для GameObject-обгорток над VFX (Particle System).
// Корисний коли VFX-префаб є third-party і має Stop Action = Destroy на своєму корені:
// дочірній VFX знищується, але обгортка залишається в Hierarchy. Цей скрипт її прибирає.
public class VfxAutoDestroy : MonoBehaviour
{
    [Tooltip("Час життя об'єкта у секундах. -1 = авто (за тривалістю найдовшого дочірнього ParticleSystem + safety buffer).")]
    [SerializeField] private float _lifetime = -1f;

    [Tooltip("Додатковий запас часу понад розрахункову тривалість (на trail/dissolve хвости, тощо).")]
    [SerializeField, Min(0f)] private float _safetyBuffer = 0.2f;

    private void Start()
    {
        float duration = _lifetime;

        // авто-режим: беремо максимум серед усіх дочірніх ParticleSystem
        if (duration < 0f)
        {
            duration = 0f;
            ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>(includeInactive: true);
            foreach (var ps in systems)
            {
                // оцінка life: duration самого емітера + максимальний lifetime частинки
                float candidate = ps.main.duration + ps.main.startLifetime.constantMax;
                if (candidate > duration)
                {
                    duration = candidate;
                }
            }
        }

        Destroy(gameObject, duration + _safetyBuffer);
    }
}
