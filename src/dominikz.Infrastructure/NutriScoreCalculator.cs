using System.Diagnostics.CodeAnalysis;
using dominikz.Domain.Enums.Cookbook;

namespace dominikz.Infrastructure;

[SuppressMessage("ReSharper", "InvalidXmlDocComment")]
public static class NutriScoreCalculator
{
    private static readonly NutriScoreValue[] ScoreToLetter = { NutriScoreValue.A, NutriScoreValue.B, NutriScoreValue.C, NutriScoreValue.D, NutriScoreValue.E };
    private static readonly decimal[] EnergyLevels = { 3350m, 3015, 2680, 2345, 2010, 1675, 1340, 1005, 670, 335 };
    private static readonly decimal[] SugarsLevels = { 45, 40, 36, 31, 27, 22.5m, 18, 13.5m, 9, 4.5m };
    private static readonly decimal[] SaturatedFattyAcidsLevels = { 10m, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
    private static readonly decimal[] SodiumLevels = { 900m, 810, 720, 630, 540, 450, 360, 270, 180, 90 };
    private static readonly decimal[] FibreLevels = { 4.7m, 3.7m, 2.8m, 1.9m, 0.9m };
    private static readonly decimal[] ProteinLevels = { 8, 6.4m, 4.8m, 3.2m, 1.6m };
    private static readonly decimal[] EnergyLevelsBeverage = { 270m, 240, 210, 180, 150, 120, 90, 60, 30, 0 };
    private static readonly decimal[] SugarsLevelsBeverage = { 13.5m, 12, 10.5m, 9, 7.5m, 6, 4.5m, 3, 1.5m, 0 };

    /// <summary>
    /// Converts energy density from kcal to EnergyKJ
    /// </summary>
    /// <returns>kJ/100g</returns>
    public static decimal GetEnergyFromKcal(decimal kcal)
        => kcal * 4.184m;

    /// <summary>
    /// SodiumFromSalt converts salt mg/100g content to sodium content 
    /// </summary>
    /// <param name="saltMg">mg/100g</param>
    /// <returns>mg/100g</returns>
    public static decimal GetSodiumFromSalt(decimal saltMg)
        => saltMg / 2.5m;

    /// <param name="energyKJ">kJ/100g</param>
    private static int CalculateByEnergy(ScoreType type, decimal energyKJ)
    {
        var steps = type == ScoreType.Beverage
            ? EnergyLevelsBeverage
            : EnergyLevels;

        return GetPointsFromRange(energyKJ, steps);
    }

    /// <param name="sugarGram">grams/100g</param>
    private static int CalculateBySugar(ScoreType type, decimal sugarGram)
    {
        var steps = type == ScoreType.Beverage
            ? SugarsLevelsBeverage
            : SugarsLevels;

        return GetPointsFromRange(sugarGram, steps);
    }

    /// <param name="saturatedFattyAcidsGram">grams/100g</param>
    private static int CalculateBySaturatedFattyAcids(decimal saturatedFattyAcidsGram)
        => GetPointsFromRange(saturatedFattyAcidsGram, SaturatedFattyAcidsLevels);

    /// <param name="sodium">mg/100g</param>
    private static int CalculateBySodium(decimal sodium)
        => GetPointsFromRange(sodium, SodiumLevels);

    /// <param name="fruitsPercent">Fruits, vegetables, pulses, nuts, and rapeseed, walnut and olive oils in percentage</param>
    private static int CalculateByFruits(ScoreType type, decimal fruitsPercent)
    {
        if (type == ScoreType.Beverage)
        {
            if (fruitsPercent > 80)
                return 10;
            if (fruitsPercent > 60)
                return 4;
            if (fruitsPercent > 40)
                return 2;
            return 0;
        }

        if (fruitsPercent > 80)
            return 5;
        if (fruitsPercent > 60)
            return 2;
        if (fruitsPercent > 40)
            return 1;
        return 0;
    }

    /// <param name="fibreGram">grams/100g</param>
    private static int CalculateByFibre(decimal fibreGram)
        => GetPointsFromRange(fibreGram, FibreLevels);

    /// <param name="proteinGram">grams/100g</param>
    private static int CalculateByProtein(decimal proteinGram)
        => GetPointsFromRange(proteinGram, ProteinLevels);

    private static int CalculateByRequest(NutriScoreRequest request)
    {
        if (request.Type == ScoreType.Water)
            return 0;

        var fruitPoints = CalculateByFruits(request.Type, request.FruitsPercent);
        var fibrePoints = CalculateByFibre(request.FibreGram);
        var energyPoints = CalculateByEnergy(request.Type, request.EnergyKJ);
        var sugarPoints = CalculateBySugar(request.Type, request.SugarGram);
        var saturatedFattyAcidsPoints = CalculateBySaturatedFattyAcids(request.SaturatedFattyAcidsGram);
        var sodiumPoints = CalculateBySodium(request.SodiumMilligram);
        var proteinPoints = CalculateByProtein(request.ProteinGram);
        var negative = energyPoints + sugarPoints + saturatedFattyAcidsPoints + sodiumPoints;
        var positive = fruitPoints + fibrePoints + proteinPoints;

        if (request.Type == ScoreType.Cheese)
            return negative - positive;

        if (negative >= 11 && fruitPoints < 5)
            return negative - fibrePoints - fruitPoints;

        return negative - positive;
    }

    public static NutriScoreValue Calculate(NutriScoreRequest request)
    {
        if (request.Type == ScoreType.Water)
            return ScoreToLetter[0];

        var value = CalculateByRequest(request);
        if (request.Type == ScoreType.Food)
            return ScoreToLetter[GetPointsFromRange(value, new[] { 18m, 10, 2, -1 })];

        return ScoreToLetter[GetPointsFromRange(value, new[] { 9m, 5, 1, -2 })];
    }

    private static int GetPointsFromRange(decimal v, decimal[] steps)
    {
        for (var i = 0; i < steps.Length; i++)
        {
            var l = steps[i];
            if (v > l)
                return steps.Length - i;
        }

        return 0;
    }
}

public record NutriScoreRequest(ScoreType Type, decimal EnergyKJ, decimal SugarGram, decimal SaturatedFattyAcidsGram, decimal SodiumMilligram, decimal FruitsPercent, decimal FibreGram, decimal ProteinGram);

public enum ScoreType
{
    Food,
    Beverage,
    Water,
    Cheese
}