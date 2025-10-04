import csv
import io

input_path = r"c:\Users\light\Documents\GitHub\PeakMods\src\PeakChatOps\Localization.csv"
output_path = r"c:\Users\light\Documents\GitHub\PeakMods\artifacts\localization_fixed.csv"
backup_path = r"c:\Users\light\Documents\GitHub\PeakMods\src\PeakChatOps\Localization.csv.bak"

# header as provided by user
header = "CURRENT_LANGUAGE,English,Français,Italiano,Deutsch,Español (España),Español (LatAm),Português (BR),Русский,Українська,简体中文,繁体中文,日本語,한국어,Polski,Türkçe,ENDLINE"
expected_cols = len(header.split(','))

with open(input_path, 'r', encoding='utf-8-sig', newline='') as f:
    raw_lines = f.readlines()

# backup original
with open(backup_path, 'w', encoding='utf-8') as b:
    b.writelines(raw_lines)

out_lines = []
# write header first
out_lines.append(header + "\n")

for raw in raw_lines:
    if raw.strip() == '':
        out_lines.append('\n')
        continue
    if raw.lstrip().startswith('#'):
        # preserve comment lines as-is
        out_lines.append(raw)
        continue
    # Try to parse this line as CSV
    reader = csv.reader([raw])
    rows = list(reader)
    if not rows:
        out_lines.append(raw)
        continue
    row = rows[0]
    if len(row) == 0:
        out_lines.append('\n')
        continue
    key = row[0]
    translations = row[1:]
    # Normalize: ensure translations length == expected_cols - 1
    target_trans_count = expected_cols - 1
    if len(translations) <= target_trans_count:
        translations = translations + [''] * (target_trans_count - len(translations))
    else:
        # merge extras into last column
        head = translations[:target_trans_count-1]
        last = ','.join(translations[target_trans_count-1:])
        translations = head + [last]
    # write using csv to ensure proper quoting
    buf = io.StringIO()
    writer = csv.writer(buf, quoting=csv.QUOTE_MINIMAL)
    writer.writerow([key] + translations)
    out_lines.append(buf.getvalue())

with open(output_path, 'w', encoding='utf-8', newline='') as out:
    out.writelines(out_lines)

print(f'Wrote fixed CSV to {output_path}, backup original to {backup_path}, expected cols = {expected_cols}')
