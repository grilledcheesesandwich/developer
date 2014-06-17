# This file creates a separate OACR project for the test code under this directory.
!IF "$(_TGTOS)" == "MC"
! INCLUDE $(_WINPHONEROOT)\src\baseos\test\subproject.inc
!ENDIF
! INCLUDE $(_WINPHONEROOT)\src\baseos\project.mk

