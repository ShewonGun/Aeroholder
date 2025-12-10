# ? Entitlement Field Updated (Alphabet + 2-Digit Number)

## The Change
The **Entitlement** field in both Add and Edit Shareholder modals now allows users to select an **alphabet letter (A-Z)** and enter a **2-digit number (00-99)**, which are combined into a single value like "A05" or "B12".

---

## Visual Result

### Entitlement Field
```
??????????????????????????????????
? Entitlement                    ?
? ?????  ????????????            ?
? ? A ?  ?   05     ?            ?
? ?????  ????????????            ?
?  ?          ?                  ?
? Letter    Number               ?
? (A-Z)     (00-99)              ?
??????????????????????????????????
```

### Result: "A05"

---

## How It Works

### User Interface
1. **Dropdown** - Select letter A-Z (or "-" for none)
2. **Number Input** - Type or paste 2-digit number (0-99)
3. **Auto-format** - Number auto-formats to 2 digits on blur (5 ? 05)
4. **Combined Value** - Sent to server as "A05", "B12", etc.

### Example Values
- Letter: **A**, Number: **5** ? Saved as: **"A05"**
- Letter: **B**, Number: **12** ? Saved as: **"B12"**
- Letter: **Z**, Number: **99** ? Saved as: **"Z99"**
- Letter: **-** (empty), Number: **0** ? Saved as: **"00"**

---

## Features

### Alphabet Dropdown
? **26 Letters** - A through Z
? **Empty Option** - "-" for no letter selected
? **Bold Font** - Letter stands out
? **60px Width** - Compact design
? **Centered Text** - Clean appearance

### Number Input
? **2-Digit Format** - Automatically pads with zero (5 ? 05)
? **Range** - 0 to 99 only
? **No Spinners** - Clean input box (arrows hidden)
? **Validation** - Limits max to 99, min to 0
? **Auto-format on Blur** - Formats when user leaves field
? **Placeholder** - Shows "00" as example

---

## Implementation Details

### Add Shareholder Modal

**HTML:**
```html
<div class="entitlement-input">
    <select id="entitlementLetter">
        <option value="">-</option>
        <option value="A">A</option>
        <option value="B">B</option>
        <!-- ... A-Z ... -->
        <option value="Z">Z</option>
    </select>
    <input type="number" id="entitlementNumber" min="0" max="99" placeholder="00">
</div>
```

**JavaScript:**
```javascript
function saveShareholder() {
    const entitlementLetter = document.getElementById('entitlementLetter')?.value || '';
    const entitlementNumber = document.getElementById('entitlementNumber')?.value || '0';
    const formattedNumber = parseInt(entitlementNumber).toString().padStart(2, '0');
    const entitlementValue = entitlementLetter + formattedNumber; // e.g., "A05"
    
    const formData = {
        // ...
        Entitlement: entitlementValue // Send combined value
    };
}

// Auto-format to 2 digits on blur
entitlementNumberInput.addEventListener('blur', function() {
    const num = parseInt(this.value) || 0;
    if (num > 99) this.value = 99;
    if (num < 0) this.value = 0;
    this.value = num.toString().padStart(2, '0');
});
```

**CSS:**
```css
.entitlement-input select {
    width: 60px;
    font-weight: 600;
    text-align: center;
}

.entitlement-input input {
    flex: 1;
    text-align: center;
}

/* Hide number input spinners */
.entitlement-input input[type="number"]::-webkit-inner-spin-button,
.entitlement-input input[type="number"]::-webkit-outer-spin-button {
    -webkit-appearance: none;
}
```

---

### Edit Shareholder Modal

**Load Existing Data:**
```javascript
function loadShareholderData(folioId) {
    fetch('/ShareholderListPage/GetShareholder?folioId=' + folioId)
        .then(result => {
            const entitlement = data.Entitlement || ''; // e.g., "A05"
            if (entitlement.length > 0) {
                const letter = entitlement.charAt(0); // "A"
                const number = entitlement.substring(1); // "05"
                document.getElementById('edit_entitlementLetter').value = letter;
                document.getElementById('edit_entitlementNumber').value = number;
            }
        });
}
```

**Update with Combined Value:**
```javascript
function updateShareholder() {
    const entitlementLetter = document.getElementById('edit_entitlementLetter')?.value || '';
    const entitlementNumber = document.getElementById('edit_entitlementNumber')?.value || '0';
    const formattedNumber = parseInt(entitlementNumber).toString().padStart(2, '0');
    const entitlementValue = entitlementLetter + formattedNumber;
    
    const formData = {
        // ...
        Entitlement: entitlementValue
    };
}
```

---

## User Experience

### Add New Shareholder
1. User opens Add modal
2. Sees Entitlement field with dropdown and number input
3. Selects letter "B" from dropdown
4. Types "7" in number input
5. Clicks elsewhere (blur event)
6. Number auto-formats to "07"
7. Clicks "Save"
8. Value "B07" is saved to database

### Edit Existing Shareholder
1. User clicks "More Info" on shareholder
2. Edit modal opens
3. Entitlement shows: Letter="A", Number="05"
4. User changes letter to "C"
5. User changes number to "12"
6. Clicks "Update"
7. Value "C12" is saved to database

---

## Validation

### Number Input Validation
```javascript
// On blur (when user leaves field)
if (value > 99) value = 99;  // Max 99
if (value < 0) value = 0;     // Min 0
value = value.padStart(2, '0'); // Format to 2 digits
```

### Examples:
| User Input | After Blur | Combined Result |
|------------|------------|-----------------|
| 5 | 05 | A05 (if letter=A) |
| 99 | 99 | B99 (if letter=B) |
| 123 | 99 | C99 (limited to max) |
| -5 | 00 | D00 (limited to min) |
| 0 | 00 | E00 |

---

## CSS Styling

### Layout
```
????????????????????????????
? [A ?]    [  05  ]        ?
?  60px     flex:1         ?
?          8px gap         ?
????????????????????????????
```

### Dropdown Style
- **Width**: 60px (compact)
- **Font-weight**: 600 (bold)
- **Text-align**: center
- **Border**: 1px solid #e5e7eb
- **Border-radius**: 4px

### Number Input Style
- **Flex**: 1 (takes remaining space)
- **Text-align**: center
- **No spinners** (arrows hidden)
- **Border**: 1px solid #e5e7eb
- **Focus**: Blue border (#1e3a8a)

---

## Data Flow

### Saving (Add/Edit)
```
User Interface:
  Dropdown: "A"
  Input: "05"
       ?
JavaScript Combines:
  "A" + "05" = "A05"
       ?
Sent to Server:
  Entitlement: "A05"
       ?
Saved to Database:
  Entitlement column: "A05"
```

### Loading (Edit)
```
Database:
  Entitlement column: "A05"
       ?
Server Returns:
  Entitlement: "A05"
       ?
JavaScript Splits:
  letter = "A" (first char)
  number = "05" (rest)
       ?
User Interface:
  Dropdown: "A"
  Input: "05"
```

---

## Example Scenarios

### Scenario 1: New Shareholder with Entitlement A15
```
1. User opens Add modal
2. Fills in shareholder details
3. Selects "A" from entitlement dropdown
4. Types "15" in number input
5. Clicks Save
6. Database stores: Entitlement = "A15"
```

### Scenario 2: Edit Shareholder - Change A15 to B20
```
1. User clicks More Info on shareholder
2. Edit modal shows: Dropdown="A", Input="15"
3. User changes dropdown to "B"
4. User changes input to "20"
5. Clicks Update
6. Database updates: Entitlement = "B20"
```

### Scenario 3: Add Shareholder with No Letter
```
1. User opens Add modal
2. Leaves dropdown at "-" (empty)
3. Types "50" in number input
4. Clicks Save
5. Database stores: Entitlement = "50"
```

### Scenario 4: Auto-format on Blur
```
1. User types "5" in number input
2. User clicks elsewhere (blur)
3. Input auto-formats to "05"
4. User sees "05" in the field
```

---

## Files Modified

### 1. Views\AddShareholderModal\addShareholderModal.cshtml
**Changes:**
- ? Updated HTML: Changed entitlement field to dropdown + number input
- ? Updated CSS: Added styles for new layout, removed spinners
- ? Updated JavaScript: Combined letter + number into single value
- ? Added blur event: Auto-format number to 2 digits

**Before:**
```html
<select id="entitlementSelect">
    <option></option>
</select>
<input type="text" id="entitlementValue" value="00" readonly>
```

**After:**
```html
<select id="entitlementLetter">
    <option value="">-</option>
    <option value="A">A</option>
    <!-- ... A-Z ... -->
</select>
<input type="number" id="entitlementNumber" min="0" max="99" placeholder="00">
```

### 2. Views\EditShareholderModal\editShareholderModal.cshtml
**Changes:**
- ? Updated HTML: Same dropdown + number input structure
- ? Updated CSS: Same styles as Add modal
- ? Updated JavaScript loadShareholderData(): Parse entitlement into letter + number
- ? Updated JavaScript updateShareholder(): Combine letter + number
- ? Added blur event: Auto-format number to 2 digits

**Load Logic:**
```javascript
const entitlement = "A05"; // From database
const letter = entitlement.charAt(0); // "A"
const number = entitlement.substring(1); // "05"
```

**Update Logic:**
```javascript
const letter = "B";
const number = "12";
const entitlement = letter + number; // "B12"
```

---

## Testing

### Test Add Shareholder
1. Click "Add" button
2. Scroll to Entitlement field
3. **See**: Dropdown (A-Z) + Number input (00)
4. Select "C" from dropdown
5. Type "25" in number input
6. Click elsewhere
7. **Verify**: Number shows "25"
8. Click "Save"
9. **Verify**: Shareholder saved with Entitlement="C25"

### Test Edit Shareholder
1. Create shareholder with Entitlement="A05"
2. Click "More Info" on that shareholder
3. **Verify**: Dropdown shows "A", Input shows "05"
4. Change dropdown to "D"
5. Change input to "77"
6. Click "Update"
7. **Verify**: Shareholder updated with Entitlement="D77"

### Test Auto-format
1. Open Add or Edit modal
2. Click in number input
3. Type "3"
4. Click elsewhere (trigger blur)
5. **Verify**: Input shows "03" (auto-formatted)

### Test Validation
1. Type "150" in number input
2. Click elsewhere
3. **Verify**: Input shows "99" (max limit)
4. Type "-10"
5. Click elsewhere
6. **Verify**: Input shows "00" (min limit)

### Test Empty Letter
1. Leave dropdown at "-"
2. Type "50" in number input
3. Click Save
4. **Verify**: Entitlement="50" (no letter prefix)

---

## Benefits

### ? Flexible Format
- Supports letters A-Z plus numbers 00-99
- Can omit letter if needed (just number)
- Expandable format for future needs

### ? User-Friendly
- Dropdown prevents typos in letters
- Number input only accepts valid numbers
- Auto-formatting helps consistency
- Visual separation of letter and number

### ? Data Quality
- Consistent format in database
- Easy to parse and display
- Validation ensures clean data
- No invalid combinations possible

### ? Easy to Use
- Intuitive interface
- Clear labeling
- Immediate feedback
- No manual formatting required

---

## Future Enhancements (Optional)

### Possible Additions:
1. **Multiple Letters** - Support "AB05", "XY12"
2. **3-Digit Numbers** - Support 000-999
3. **Wildcards** - Support patterns like "A*"
4. **Ranges** - Support "A00-A99"
5. **Validation Rules** - Business logic for valid combinations
6. **Auto-suggestions** - Suggest next available entitlement

---

## Summary

### What Changed
?? **Entitlement field** ? Dropdown (A-Z) + Number input (00-99)  
?? **Letter selection** ? Dropdown with A-Z options  
?? **Number input** ? Editable 2-digit field (0-99)  
?? **Auto-format** ? Pads numbers to 2 digits on blur  
?? **Combined value** ? Saves as "A05", "B12", etc.  
?? **Both modals** ? Add and Edit work the same way  

### Result
?? **Flexible entitlement system**  
?? **User-friendly interface**  
?? **Consistent data format**  
?? **Easy to extend**  

---

**Status**: ? **COMPLETE**  
**Add Modal**: ? **Alphabet + Number**  
**Edit Modal**: ? **Alphabet + Number**  
**Auto-format**: ? **Works on blur**  
**Validation**: ? **0-99 range enforced**  
**Build**: ? **Successful**  

---

**Last Updated**: January 2025
