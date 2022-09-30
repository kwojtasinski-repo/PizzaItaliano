import { isEmpty } from "./stringExtensions";

export function validateEmail(text) {
    const pattern = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return pattern.test(text);
}

export function validatePassword(text) {
    const pattern = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d\\w\\W]{8,}$");
    return pattern.test(text);
}

export function validate(rules = [], value) {
    for (let i = 0; i < rules.length; i++) {
        const rule = rules[i];

        if (rule instanceof Object) {
            const errorMessage = availableRules[rule.rule](value, rule);
            if (errorMessage) {
                return errorMessage;
            }
        } else {
            const errorMessage = availableRules[rule](value);
            if (errorMessage) {
                return errorMessage;
            }
        }
    }

    return '';
}

const availableRules = {
    required(value) {
        return value ? '' : 'Field is required';
    },
    min(value, rule) {
      return value.length >= rule.length ? '' : `Min. characters: ${rule.length}`;
    },
    email(value) {
      return validateEmail(value) ? '' : 'Invalid email';
    },
    password(value) {
        return validatePassword(value) ? '' : 'Password should contain at least 8 characters, including one upper letter and one number';
    },
    only(value, rule) {
        return value.length === rule.length ? '' : `Field should contain ${rule.length} characters`;
    },
    greaterOrEqualTo(value, rule) {
        const localValue = Number(value);
        if(!isNumber(localValue)) {
            return;
        }

        if(!isNumber(rule.equalityValue)) {
            return;
        }

        return Number(localValue) >= Number(rule.equalityValue) ? '' : `Field should be greater or equal to ${rule.equalityValue}`;
    },
    greaterThan(value, rule) {
        const localValue = Number(value);
        if(!isNumber(localValue)) {
            return;
        }

        if(!isNumber(rule.equalityValue)) {
            return;
        }

        return Number(localValue) > Number(rule.equalityValue) ? '' : `Field should be greater than ${rule.equalityValue}`;
    },
    lessOrEqualTo(value, rule) {
        const localValue = Number(value);
        if(!isNumber(localValue)) {
            return;
        }

        if(!isNumber(rule.qualityValue)) {
            return;
        }

        return Number(localValue) <= Number(rule.equalityValue) ? '' : `Field should be less or equal to ${rule.equalityValue}`;
    },
    lessThan(value, rule) {
        const localValue = Number(value);
        if(!isNumber(localValue)) {
            return;
        }

        if(!isNumber(rule.equalityValue)) {
            return;
        }

        return Number(localValue) < Number(rule.equalityValue) ? '' : `Field should be less than ${rule.equalityValue}`;
    },
    requiredIf(value, rules) {
        if (rules.isRequired) {
            let errorMessage = '';
            
            if (isEmpty(value)) {
                errorMessage = 'Field is required';
                return errorMessage;
            }

            if (rules.rules) {
                const rulesInside = rules.rules;
                for(const key in rulesInside) {
                    const rule = rulesInside[key];
                    errorMessage = availableRules[rule.rule](value, rule);

                    if (errorMessage) {
                        return errorMessage;
                    }
                }
            }
        }
    }
};

function isNumber(value) {
    return typeof value === 'number' && !Number.isNaN(value);
}