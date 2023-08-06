export enum ValueState {
    Good,
    Questionable,
    Dangerous,
    Unknown
}

export function getTemperatureValueState(valueString: string): ValueState {
    if (!valueString || valueString === '') {
        return ValueState.Unknown;
    }

    const value = parseFloat(valueString);

    if (!value) {
        return ValueState.Unknown;
    }

    if (value > 23 && value < 26) {
        return ValueState.Good;
    }

    if (value > 22 && value < 27) {
        return ValueState.Questionable;
    }

    return ValueState.Dangerous;
}