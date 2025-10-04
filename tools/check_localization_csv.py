import csv
from collections import Counter

input_path = r"c:\Users\light\Documents\GitHub\PeakMods\src\PeakChatOps\Localization.csv"
report_path = r"c:\Users\light\Documents\GitHub\PeakMods\artifacts\localization_csv_report.txt"
fixes_path = r"c:\Users\light\Documents\GitHub\PeakMods\artifacts\localization_fixes.csv"

with open(input_path, 'r', encoding='utf-8-sig', newline='') as f:
    # use csv.reader to correctly handle quoted fields
    reader = csv.reader(f)
    rows = list(reader)

# compute counts
counts = Counter(len(r) for r in rows)
most_common_len, _ = counts.most_common(1)[0]

with open(report_path, 'w', encoding='utf-8') as rep:
    rep.write(f"Detected {len(rows)} rows\n")
    rep.write(f"Field counts frequency:\n")
    for length, freq in sorted(counts.items()):
        rep.write(f"  {length} fields: {freq} rows\n")
    rep.write(f"Assuming correct field count = {most_common_len}\n\n")

    # list offending rows
    bad_rows = []
    for i, r in enumerate(rows, start=1):
        if len(r) != most_common_len:
            bad_rows.append((i, len(r), r))
            rep.write(f"Row {i}: {len(r)} fields\n")
            rep.write(','.join(r) + '\n')

with open(fixes_path, 'w', encoding='utf-8', newline='') as out:
    writer = csv.writer(out, quoting=csv.QUOTE_MINIMAL)
    for i, length, r in bad_rows:
        # Suggest a quoted version: join original raw line by comma and write as a single field for safety
        raw_line = ','.join(r)
        # produce a row where we put the first column (key) then quote the rest as a single field
        if len(r) >= 2:
            key = r[0]
            rest = ','.join(r[1:])
            writer.writerow([key, rest])
        else:
            writer.writerow(r)

print(f"Report written to {report_path}")
print(f"Fixes written to {fixes_path} (one-line-per-offending-row with key + quoted remainder)")
