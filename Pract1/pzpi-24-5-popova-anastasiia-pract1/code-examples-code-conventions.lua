-- Запуск: lua Lua_Code_Conventions_Pract1_All_In_One.lua.

local function show_section(number, title)
    print("\n" .. number .. ". " .. title)
end


-- package.preload дає змогу продемонструвати окремі модулі.
package.preload["modules.calculator"] = function()
    local calculator = {}

    function calculator.add(a, b)
        return a + b
    end

    function calculator.multiply(a, b)
        return a * b
    end

    return calculator
end

local calculator = require("modules.calculator")

show_section(4, "Модулі")
print("Додавання:", calculator.add(5, 3))
print("Множення:", calculator.multiply(5, 3))


-- Форматування коду.
show_section(5, "Форматування")

-- Небажано: навмисно пропущено відступи.
function check_age(age)
if age >= 18 then
print("Adult")
else
print("Minor")
end
end

check_age(18)
_G.check_age = nil

-- Рекомендовано: чотири пробіли на рівень вкладеності.
local function check_age(age)
    if age >= 18 then
        print("Adult")
    else
        print("Minor")
    end
end

check_age(18)


-- Іменування змінних та функцій.
show_section(6, "Іменування")

-- Небажано: з назв не зрозуміло призначення значень.
local x = 100
local y = 0.2
local z = x * (1 - y)

print("Неінформативні назви:", z)

-- Рекомендовано: змістовні назви у стилі snake_case.
local product_price = 100
local discount_rate = 0.2
local final_price = product_price * (1 - discount_rate)

print("Змістовні назви:", final_price)

-- Приклади угод про іменування.
local camelCase = "productPrice"
local PascalCase = "ProductPrice"
local MAX_ATTEMPTS = 3

print("camelCase:", camelCase, "PascalCase:", PascalCase)
print("Іменована константа:", MAX_ATTEMPTS)


-- Коментарі.
show_section(7, "Коментарі")

-- Надлишковий коментар:
local counter = 0
-- Збільшуємо лічильник на один.
counter = counter + 1

-- Корисний коментар:
-- Залишаємо одну додаткову спробу
-- для повторного введення даних.
local max_attempts = 4

print("Лічильник:", counter)
print("Кількість спроб:", max_attempts)


-- Документування функцій.
package.preload["modules.pricing"] = function()
    local pricing = {}

    --- Обчислює кінцеву вартість товару.
    -- @param price Початкова вартість.
    -- @param discount Частка знижки.
    -- @return Вартість після знижки.
    function pricing.calculate_price(price, discount)
        return price * (1 - discount)
    end

    return pricing
end

local pricing = require("modules.pricing")

show_section(8, "Документування")
print("Ціна зі знижкою:", pricing.calculate_price(200, 0.1))


-- Локальні та глобальні змінні.
show_section(9, "Область видимості")

-- Небажано: глобальна змінна та глобальна функція.
total = 0

function add_value(value)
    total = total + value
end

add_value(5)
print("Глобальна змінна:", total)
_G.total = nil
_G.add_value = nil

-- Рекомендовано: обмеження області видимості через local.
local total = 0

local function add_value(value)
    total = total + value
end

add_value(5)
print("Локальна змінна:", total)


-- Тестування та автоматичні перевірки.
-- TDD: Red (тест не проходить) -> Green (реалізація) ->
-- Refactor (поліпшення без зміни поведінки).
show_section(10, "Перевірки")

assert(calculator.add(5, 3) == 8)
assert(calculator.multiply(5, 3) == 15)
assert(pricing.calculate_price(200, 0.1) == 180)

print("Базові перевірки пройдено")

-- Зразок тесту Busted із презентації.
-- Виконується лише в середовищі, де доступні функції Busted.
if type(describe) == "function" and type(it) == "function" then
    describe("Calculator", function()
        it("adds two numbers", function()
            assert.are.equal(8, calculator.add(5, 3))
        end)
    end)
end


-- Код до та після покращення.
show_section(11, "Порівняння коду")

-- До покращення: погані назви, відсутність відступів,
-- глобальна функція та «магічне» число.
function f(x,y)
if x>100 then
return x-x*y
else
return x
end
end

local result_before = f(200, 0.1)
_G.f = nil

-- Після покращення: ті самі вхідні дані та поведінка.
local MIN_DISCOUNT_PRICE = 100

local function calculate_final_price(price, discount_rate)
    if price > MIN_DISCOUNT_PRICE then
        return price * (1 - discount_rate)
    end

    return price
end

local result_after = calculate_final_price(200, 0.1)
print("До покращення:", result_before)
print("Після покращення:", result_after)

assert(result_before == result_after and result_after == 180)

print("\nУсі демонстраційні приклади виконано успішно.")
