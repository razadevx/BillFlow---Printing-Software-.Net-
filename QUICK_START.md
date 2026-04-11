# BillFlow Quick Start Guide (5 Minutes)

**This guide will get you started with BillFlow in 5 minutes.**

---

## Step 1: Launch BillFlow (1 minute)

1. Double-click **BillFlow.exe**
2. Application starts automatically
3. Database initializes on first run (30 seconds)
4. Main dashboard appears

---

## Step 2: Add Company Information (1 minute)

1. Click **Settings** in the left sidebar
2. Enter your company details:
   - Company Name
   - Phone Number
   - Email Address
   - Address
3. Set **Default Rate** (e.g., ₹50 per SqFt)
4. Set **Tax Rate** (if applicable, e.g., 5%)
5. Click **Save**

---

## Step 3: Add Your First Customer (1 minute)

1. Click **Customers** in sidebar
2. Click **New Customer** button
3. Fill in details:
   - Name (required)
   - Phone (10 digits, e.g., 9876543210)
   - Email (optional)
   - Address (optional)
   - Credit Limit (optional, e.g., ₹50,000)
4. Click **Save**
5. Customer code auto-generates

---

## Step 4: Create Your First Work Order (1 minute)

1. Click **Work Orders**
2. Click **New Work Order**
3. Select customer from dropdown
4. Add line items:
   - Description (e.g., "Poster Printing")
   - Width: 14.3
   - Height: 13
   - Quantity: 1
   - Rate will auto-fill from settings
   - **SqFt automatically calculates** to 185.9
5. Click **Add Item**
6. Total price shows below items
7. Click **Save**

---

## Step 5: View Dashboard (1 minute)

1. Click **Dashboard**
2. See:
   - ✅ Total pending amount
   - ✅ Today's work items
   - ✅ Top customers by credit
   - ⚠️ Active credit alerts
3. Daily schedule: Use arrows to navigate dates

---

## Common Tasks

### Mark Work as Complete

1. Go to **Daily Schedule**
2. Select date using arrows
3. Under "Work To-Do", click work item
4. Click **Mark Complete**
5. Status updates to "Completed"

### Record Payment

1. Go to **Digital Khata**
2. Click customer name
3. Click **Add Payment**
4. Enter amount
5. Click **Save**
6. Balance updates automatically

### Send Invoice

1. Go to **Invoices**
2. Select work order
3. Click **Generate Invoice**
4. Click **Download PDF**
5. Or click **Send SMS** to customer

### View Credit Report

1. Go to **Reports & Analytics**
2. See credit health summary:
   - Customers with clear balance (🟢)
   - Customers with moderate balance (🟡)
   - High-risk customers (🔴)
3. Click **Export CSV** for spreadsheet

### Create Backup

1. Go to **Settings**
2. Click **Create Backup**
3. Backup saved to:
   - `%APPDATA%\BillFlow\Backups\`
4. Automatic daily backups also enabled

---

## Important Settings

### Enable/Disable Features

**Settings** → Look for:
- ✅ **Enable Credit** - Track customer credit
- ✅ **Enable Auto-Backup** - Daily automatic backups
- ✅ **Enable Offline Mode** - Work without internet
- ✅ **Enable Credit Alerts** - Get alerts for risky accounts

---

## Quick Keyboard Shortcuts

| Action | Shortcut |
|--------|----------|
| Save | Ctrl + S |
| New Item | Ctrl + N |
| Refresh | F5 |
| Print | Ctrl + P |

---

## Understanding the Dashboard Numbers

| Metric | Meaning |
|--------|---------|
| **₹17,095** | Total pending amount across all customers |
| **341.9 SqFt** | Total work in current order |
| **5 Customers** | Customers with pending balance |
| **🟢 Clear** | No balance or paid within 7 days |
| **🟡 Moderate** | Has balance, paid within 30 days |
| **🔴 High Risk** | >30 days overdue or over credit limit |

---

## Calculations Made Automatic

### Example Scenario

**Customer:** ABC Printing  
**Order:** Poster printing

| Field | Value |
|-------|-------|
| Width | 14.3 cm |
| Height | 13 cm |
| Quantity | 1 |
| **SqFt (Auto)** | **185.9** |
| Rate/SqFt | ₹50 |
| **Cost (Auto)** | **₹9,295** |
| Tax (5%) | ₹464.75 |
| **Total (Auto)** | **₹9,759.75** |

**You only enter:** Width, Height, Quantity, Rate  
**AutoCalculated:** SqFt, Cost, Tax, Total

---

## Troubleshooting Quick Tips

| Issue | Solution |
|-------|----------|
| App won't start | Verify .NET 8.0 installed |
| Database locked | Close other instances, wait 30s |
| Numbers show 0 | Check Settings → Default Rate |
| Customer not found | Create customer first (Step 3) |
| Can't send SMS | Check phone has country code (+91) |
| Backup failed | Check disk space (200 MB min) |

---

##View Your Data

### Where is my data?

- **Database:** `%LOCALAPPDATA%\BillFlow\billflow.db`
- **Logs:** `%APPDATA%\BillFlow\Logs\`
- **Backups:** `%APPDATA%\BillFlow\Backups\`

### Access these folders:

1. Press **Windows Key + R**
2. Type: `%APPDATA%\BillFlow`
3. **Enter**

---

## Recommended Workflow

### Daily

1. ☐ Open BillFlow
2. ☐ Check Dashboard for alerts
3. ☐ Update Daily Schedule (mark work done)
4. ☐ Record payments from customers

### Weekly

1. ☐ Check Digital Khata for overdue payments
2. ☐ Review credit alerts
3. ☐ Send payment reminders if needed

### Monthly

1. ☐ Export reports
2. ☐ Verify data integrity
3. ☐ Check backups completed
4. ☐ Review logs for errors

---

## Need More Help?

1. **Settings page:** Detailed help for each feature
2. **Production Guide:** `PRODUCTION_GUIDE.md`
3. **Changelog:** `CHANGELOG.md` - All new features explained
4. **Logs:** `%APPDATA%\BillFlow\Logs\` - Error diagnostics

---

## Key Advantages

✨ **No Manual Calculations**
- SqFt, costs, totals = automatic

🔐 **Safe & Backed Up**
- Daily automatic backups
- Easy restore option

📱 **Works Offline**
- Create orders without internet
- Auto-syncs when online

💾 **Smart Organization**
- Customer histories tracked
- Payment records maintained
- Invoice archive

⚠️ **Alert System**
- Automatic credit alerts
- Overdue notifications
- Balance tracking

📊 **Easy Reporting**
- Export to Excel/JSON
- Credit health reports
- Daily analytics

---

## Support Contacts

- **Documentation:** See PRODUCTION_GUIDE.md
- **Logs:** %APPDATA%\BillFlow\Logs\ (for IT support)
- **Export Data:** Reports > Export CSV

---

**Ready to get started? Go to Step 1 →**

*BillFlow makes billing simple. All calculations are automatic. You focus on business, we handle math!*
