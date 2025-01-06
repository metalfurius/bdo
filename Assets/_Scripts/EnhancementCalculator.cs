using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhancementCalculator : MonoBehaviour
{
    public TMP_InputField baseCostInput; // Precio base del objeto
    public TMP_InputField materialCostInput; // Costo de materiales por intento
    public TMP_InputField successRateInput; // Porcentaje de éxito (0-100)
    public TMP_InputField successPriceInput; // Precio del objeto mejorado
    public TMP_InputField pityInput; // Intentos máximos garantizados (opcional)
    public TMP_InputField cronsPerAttemptInput; // Número de crons por intento
    public Toggle isAccessoryToggle; // Determina si el objeto es un accesorio
    public Toggle useCronsToggle; // Determina si se usan crons
    public TMP_Text resultText;

    private const float CronCost = 3f; // Costo de cada cron en millones

    private void Start()
    {
        // Suscribir eventos onValueChanged para los campos y toggles
        baseCostInput.onValueChanged.AddListener(OnInputChanged);
        materialCostInput.onValueChanged.AddListener(OnInputChanged);
        successRateInput.onValueChanged.AddListener(OnInputChanged);
        successPriceInput.onValueChanged.AddListener(OnInputChanged);
        pityInput.onValueChanged.AddListener(OnInputChanged);
        cronsPerAttemptInput.onValueChanged.AddListener(OnInputChanged);
        isAccessoryToggle.onValueChanged.AddListener(OnToggleChanged);
        useCronsToggle.onValueChanged.AddListener(OnToggleChanged);

        resultText.text = "Introduce valores en los campos para calcular.";
    }

    private void OnInputChanged(string value)
    {
        if (IsAllInputsFilled())
        {
            Calculate();
        }
        else
        {
            resultText.text = "Por favor, completa todos los campos requeridos.";
        }
    }

    private void OnToggleChanged(bool value)
    {
        if (IsAllInputsFilled())
        {
            Calculate();
        }
    }

    private bool IsAllInputsFilled()
    {
        return !string.IsNullOrEmpty(baseCostInput.text) &&
               !string.IsNullOrEmpty(materialCostInput.text) &&
               !string.IsNullOrEmpty(successRateInput.text) &&
               !string.IsNullOrEmpty(successPriceInput.text);
    }

    private void Calculate()
    {
        if (float.TryParse(baseCostInput.text, out var baseCost) &&
            float.TryParse(materialCostInput.text, out var materialCost) &&
            float.TryParse(successRateInput.text, out var successRate) &&
            float.TryParse(successPriceInput.text, out var successPrice))
        {
            successRate /= 100f;

            if (successRate <= 0f)
            {
                resultText.text = "El porcentaje de éxito debe ser mayor que 0.";
                return;
            }

            var isAccessory = isAccessoryToggle.isOn;
            var useCrons = useCronsToggle.isOn;
            var pity = 0;
            if (!string.IsNullOrEmpty(pityInput.text) && int.TryParse(pityInput.text, out var pityValue))
            {
                pity = pityValue;
            }

            var cronsPerAttempt = 0;
            if (!string.IsNullOrEmpty(cronsPerAttemptInput.text) && int.TryParse(cronsPerAttemptInput.text, out var cronValue))
            {
                cronsPerAttempt = cronValue;
            }

            var cronCostPerAttempt = cronsPerAttempt * CronCost;

            // Variables adicionales para el cálculo con crons
            const float downgradeChance = 0.4f; // 40% de posibilidad de bajar de nivel al fallar
            var effectiveAttemptsFactor = useCrons && isAccessory ? 1f / (1f - downgradeChance) : 1f;

            // Mejor caso: Éxito en el primer intento
            var bestCaseCost = baseCost + materialCost + (useCrons ? cronCostPerAttempt : 0);
            var bestCaseProfit = successPrice - bestCaseCost;

            // Promedio: Basado en intentos esperados (1 / successRate)
            var averageAttempts = 1f / successRate * effectiveAttemptsFactor;
            var averageCost = (averageAttempts * materialCost) +
                              (useCrons 
                                  ? averageAttempts * cronCostPerAttempt + baseCost
                                  : isAccessory 
                                      ? averageAttempts * baseCost 
                                      : baseCost);
            var averageProfit = successPrice - averageCost;

            // Peor caso
            float worstCaseCost;
            if (pity > 0) // Si hay pity, calcular con ese límite
            {
                if (useCrons) // Si se usan crons, considerar bajadas de nivel
                {
                    var worstCaseAttempts = pity * effectiveAttemptsFactor;
                    worstCaseCost = baseCost + (worstCaseAttempts * materialCost) + (worstCaseAttempts * cronCostPerAttempt);
                }
                else if (isAccessory) // Accesorio: Cada fallo implica comprar el objeto base
                {
                    worstCaseCost = (pity * baseCost) + (pity * materialCost);
                }
                else // No accesorio: Sólo materiales por intentos
                {
                    worstCaseCost = baseCost + (pity * materialCost);
                }
            }
            else // Sin pity
            {
                resultText.text = "Por favor, ingresa un valor de pity para calcular el peor caso.";
                return;
            }

            var worstCaseProfit = successPrice - worstCaseCost;

            // Mostrar resultados
            resultText.text =
                $"<b>Resultados:</b>\n" +
                $"<b>Mejor Caso:</b> Costo: {bestCaseCost:F2}M, Ganancia: {bestCaseProfit:F2}M\n" +
                $"<b>Promedio:</b> Costo: {averageCost:F2}M, Ganancia: {averageProfit:F2}M\n" +
                $"<b>Peor Caso:</b> Costo: {worstCaseCost:F2}M, Ganancia: {worstCaseProfit:F2}M";
        }
        else
        {
            resultText.text = "Por favor, ingresa valores numéricos válidos en todos los campos.";
        }
    }
}

