One folder + one asmdef per feature. See ARCHITECTURE.md. No feature asmdef
references another feature's asmdef — cross-feature communication goes
through `KidzGame.Core` DTOs/interfaces + events, not direct references.
