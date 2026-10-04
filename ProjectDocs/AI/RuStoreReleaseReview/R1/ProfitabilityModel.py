"""Hypothetical 90-day install-cohort model; no live telemetry or provider access."""
import argparse
from decimal import Decimal, ROUND_HALF_UP
import json
from pathlib import Path


FIELDS = ('installs', 'paidFraction', 'payerFraction', 'grossSpendPerPayer',
          'feeFraction', 'refundFraction', 'netAdsPerInstall', 'variableCostPerInstall',
          'paidCpi', 'allocatedFixedCost', 'allocatedDevelopmentCost')


def calculate(values):
    d = {key: Decimal(str(values[key])) for key in FIELDS}
    if any(not value.is_finite() for value in d.values()):
        raise ValueError('All inputs must be finite.')
    if d['installs'] <= 0 or d['installs'] != d['installs'].to_integral_value():
        raise ValueError('Installs must be a positive integer.')
    if any(value < 0 for value in d.values()):
        raise ValueError('Inputs must not be negative.')
    if any(d[key] > 1 for key in ('paidFraction', 'payerFraction', 'feeFraction', 'refundFraction')):
        raise ValueError('Fractions must lie between zero and one.')
    n = d['installs']
    gross = n * d['payerFraction'] * d['grossSpendPerPayer']
    realized = (1 - d['refundFraction']) * (1 - d['feeFraction'])
    net = gross * realized
    contribution = net / n + d['netAdsPerInstall'] - d['variableCostPerInstall']
    fixed = d['allocatedFixedCost'] + d['allocatedDevelopmentCost']
    marketing = n * d['paidFraction'] * d['paidCpi']
    headroom = contribution - fixed / n
    required = max(Decimal(0), d['paidFraction'] * d['paidCpi']
                   + d['variableCostPerInstall'] + fixed / n - d['netAdsPerInstall'])
    denominator = d['grossSpendPerPayer'] * realized
    conversion = required / denominator if denominator else (Decimal(0) if not required else None)
    money = lambda value: float(value.quantize(Decimal('0.01'), rounding=ROUND_HALF_UP))
    return {'grossIap': money(gross), 'netIap': money(net),
            'netIapPerInstall': money(net / n), 'contributionPerInstall': money(contribution),
            'marketingCost': money(marketing), 'preTaxCohortSurplus': money(n * contribution - marketing - fixed),
            'breakEvenPaidCpi': money(headroom / d['paidFraction']) if d['paidFraction'] else None,
            'requiredPayerFraction': float(conversion) if conversion is not None else None,
            'scope': 'Hypothetical 90-day cohort; actual tax and unentered costs excluded.'}


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('input', type=Path, help='Scenario JSON with cases[].inputs')
    args = parser.parse_args()
    data = json.loads(args.input.read_text())
    print(json.dumps([{'name': case['name'], 'outputs': calculate(case['inputs'])}
                      for case in data['cases']], indent=2))
