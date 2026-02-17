using Microsoft.AspNetCore.Mvc;
using SportDiary.ViewModels.Calculators;

namespace SportDiary.Controllers
{
    public class CalculatorsController : Controller
    {
        [HttpGet]
        public IActionResult Tdee()
        {
            var vm = new TdeeInputVm();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Tdee(TdeeInputVm input)
        {
            if (!ModelState.IsValid)
                return View(input);

            var (bmr, methodBg) = CalculateBmr(input);

            // ✅ Закръгляме TDEE първо, после правим процентите върху закръгления TDEE
            var tdeeRaw = bmr * input.ActivityMultiplier;
            var tdee = Math.Round(tdeeRaw);

            // BMI
            var heightM = input.HeightCm / 100.0;
            var bmi = input.WeightKg / (heightM * heightM);

            // Lean Body Mass (само ако има BF%)
            double? lbm = null;
            if (input.BodyFatPercent.HasValue)
            {
                var bf = input.BodyFatPercent.Value / 100.0;
                lbm = input.WeightKg * (1.0 - bf);
            }

            // Протеин ориентир (2 g/kg)
            var proteinTarget = input.WeightKg * 2.0;

            var result = new TdeeResultVm
            {
                Bmr = Math.Round(bmr),
                Tdee = tdee,

                Cut15 = Math.Round(tdee * 0.85),
                Cut20 = Math.Round(tdee * 0.80),
                Bulk10 = Math.Round(tdee * 1.10),

                Method = methodBg,

                Bmi = Math.Round(bmi, 1),
                LeanBodyMassKg = lbm.HasValue ? Math.Round(lbm.Value, 1) : null,
                ProteinTargetG = Math.Round(proteinTarget)
            };

            ViewBag.Result = result;
            return View(input);
        }


        private static (double bmr, string methodBg) CalculateBmr(TdeeInputVm input)
        {
            if (input.BodyFatPercent.HasValue)
            {
                var bf = input.BodyFatPercent.Value / 100.0;
                var lbm = input.WeightKg * (1.0 - bf);
                var bmr = 370.0 + (21.6 * lbm);

                return (bmr, "Katch–McArdle (с % мазнини)");
            }

            var baseBmr = (10.0 * input.WeightKg) + (6.25 * input.HeightCm) - (5.0 * input.Age);
            var bmr2 = input.Gender == Gender.Male ? baseBmr + 5.0 : baseBmr - 161.0;

            return (bmr2, "Mifflin–St Jeor");
        }
    }
}
